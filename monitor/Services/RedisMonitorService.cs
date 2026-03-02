using StackExchange.Redis;
using RedisMonitor.Models;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;

namespace RedisMonitor.Services;

public class RedisMonitorService : IDisposable
{
    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;
    private readonly ConcurrentDictionary<string, List<RedisMessage>> _messagesByChannel = new();
    private readonly List<RedisMessage> _allMessages = new();
    private readonly object _lockObject = new();
    private RedisSettings _settings = new();
    private CancellationTokenSource? _cts;
    private int _connectAttemptId = 0;
    private int _cancelAttemptId = -1;

    public event Action? OnMessagesChanged;
    public event Action<string>? OnConnectionStatusChanged;

    public bool IsConnected => _redis?.IsConnected ?? false;

    private void NotifyConnectionStatus(int attemptId, string status)
    {
        // Only notify if this attempt is still the current one
        if (Volatile.Read(ref _connectAttemptId) == attemptId)
        {
            OnConnectionStatusChanged?.Invoke(status);
        }
    }

    public async Task ConnectAsync(RedisSettings settings)
    {
        _settings = settings;
        var attemptId = Interlocked.Increment(ref _connectAttemptId);
        Interlocked.Exchange(ref _cancelAttemptId, -1);

        try
        {
            await DisconnectAsync();

            _cts = new CancellationTokenSource();

            var configOptions = new ConfigurationOptions
            {
                EndPoints = { { settings.Host, settings.Port } },
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            _redis = await ConnectionMultiplexer.ConnectAsync(configOptions);
            if (IsCancelRequested(attemptId))
            {
                await DisconnectAsync(notify: false);
                NotifyConnectionStatus(attemptId, "Verbindung abgebrochen");
                return;
            }

            _subscriber = _redis.GetSubscriber();

            await _subscriber.SubscribeAsync(RedisChannel.Pattern("*"), (channel, message) =>
            {
                HandleRedisMessage(channel, message);
            });

            if (IsCancelRequested(attemptId))
            {
                await DisconnectAsync(notify: false);
                NotifyConnectionStatus(attemptId, "Verbindung abgebrochen");
                return;
            }

            NotifyConnectionStatus(attemptId, $"Verbunden mit {settings.Host}:{settings.Port}");
        }
        catch (Exception ex)
        {
            // If this attempt was cancelled, swallow the exception so it doesn't
            // override a later successful connection or notify the UI with a stale error.
            if (IsCancelRequested(attemptId))
            {
                return;
            }

            // Only notify if this is still the current attempt; do not rethrow.
            NotifyConnectionStatus(attemptId, $"Fehler: {ex.Message}");
            return;
        }
    }

    private void HandleRedisMessage(RedisChannel channel, RedisValue message)
    {
        var channelName = channel.ToString();
        var payload = message.ToString();

        var formattedPayload = FormatJson(payload);

        var redisMessage = new RedisMessage
        {
            Timestamp = DateTime.Now,
            Channel = channelName,
            Payload = payload,
            FormattedPayload = formattedPayload
        };

        lock (_lockObject)
        {
            if (!_messagesByChannel.ContainsKey(channelName))
            {
                _messagesByChannel[channelName] = new List<RedisMessage>();
            }

            var messages = _messagesByChannel[channelName];
            messages.Add(redisMessage);

            // Apply retention policy per channel
            if (messages.Count > _settings.MaxMessages)
            {
                messages.RemoveAt(0);
            }

            // Also add to all messages list
            _allMessages.Add(redisMessage);

            // Apply retention policy for all messages
            if (_allMessages.Count > _settings.MaxMessages)
            {
                _allMessages.RemoveAt(0);
            }
        }

        OnMessagesChanged?.Invoke();
    }


    private JsonSerializerOptions options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private string FormatJson(string json)
    {
        try
        {
            var jsonDoc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(jsonDoc, options);
        }
        catch
        {
            return json;
        }
    }

    public List<string> GetChannels()
    {
        lock (_lockObject)
        {
            return _messagesByChannel.Keys.OrderBy(k => k).ToList();
        }
    }

    public List<RedisMessage> GetMessages(string channel)
    {
        lock (_lockObject)
        {
            return _messagesByChannel.ContainsKey(channel)
                ? new List<RedisMessage>(_messagesByChannel[channel])
                : new List<RedisMessage>();
        }
    }

    public List<RedisMessage> GetAllMessages()
    {
        lock (_lockObject)
        {
            return new List<RedisMessage>(_allMessages);
        }
    }

    public static bool MatchesFilter(RedisMessage message, string includeFilter, string excludeFilter)
    {
        var channelName = message.Channel;
        var payload = message.Payload;

        // Apply include filter
        if (!string.IsNullOrWhiteSpace(includeFilter))
        {
            var includeTerms = includeFilter.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (!includeTerms.Any(term => channelName.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                          payload.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }

        // Apply exclude filter
        if (!string.IsNullOrWhiteSpace(excludeFilter))
        {
            var excludeTerms = excludeFilter.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (excludeTerms.Any(term => channelName.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase) ||
                                         payload.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearMessages()
    {
        lock (_lockObject)
        {
            _messagesByChannel.Clear();
            _allMessages.Clear();
        }
        OnMessagesChanged?.Invoke();
    }

    public async Task CancelConnectAsync()
    {
        var attemptId = Volatile.Read(ref _connectAttemptId);
        if (attemptId <= 0) return;

        Interlocked.Exchange(ref _cancelAttemptId, attemptId);
        _cts?.Cancel();
        await DisconnectAsync(notify: false);
        OnConnectionStatusChanged?.Invoke("Verbindung abgebrochen");
    }

    private bool IsCancelRequested(int attemptId)
    {
        return Volatile.Read(ref _cancelAttemptId) == attemptId;
    }

    public async Task DisconnectAsync(bool notify = true)
    {
        _cts?.Cancel();

        if (_subscriber != null)
        {
            await _subscriber.UnsubscribeAllAsync();
        }

        if (_redis != null)
        {
            await _redis.CloseAsync();
            _redis.Dispose();
            _redis = null;
        }

        _subscriber = null;
        if (notify)
        {
            OnConnectionStatusChanged?.Invoke("Getrennt");
        }
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
        _cts?.Dispose();
    }
}