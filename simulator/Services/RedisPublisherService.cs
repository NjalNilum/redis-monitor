using RedisSimulator.Models;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RedisSimulator.Services;

public class RedisPublisherService : IDisposable
{
    private const int MaxMessages = 1000;

    private ConnectionMultiplexer? _redis;
    private ISubscriber? _subscriber;
    private readonly ConcurrentQueue<PublishedMessage> _messages = new();
    private readonly object _publishLock = new();

    private CancellationTokenSource? _cycleCts;
    private Task? _cycleTask;

    public event Action? OnMessagesChanged;
    public event Action<string>? OnConnectionStatusChanged;
    public event Action<string>? OnCycleStatusChanged;

    public bool IsConnected => _redis?.IsConnected ?? false;
    public bool IsCycling => _cycleTask is { IsCompleted: false };

    private readonly JsonSerializerOptions _prettyJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task ConnectAsync(RedisSettings settings)
    {
        try
        {
            await DisconnectAsync(notify: false);

            var options = new ConfigurationOptions
            {
                EndPoints = { { settings.Host, settings.Port } },
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            _redis = await ConnectionMultiplexer.ConnectAsync(options);
            _subscriber = _redis.GetSubscriber();

            OnConnectionStatusChanged?.Invoke($"Verbunden mit {settings.Host}:{settings.Port}");
        }
        catch (Exception ex)
        {
            OnConnectionStatusChanged?.Invoke($"Fehler: {ex.Message}");
        }
    }

    public async Task DisconnectAsync(bool notify = true)
    {
        await StopCyclicPublishAsync();

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

    public async Task PublishAsync(string channel, string payload, bool isCyclic = false)
    {
        if (_subscriber == null)
        {
            throw new InvalidOperationException("Keine Redis-Verbindung vorhanden.");
        }

        if (string.IsNullOrWhiteSpace(channel))
        {
            throw new InvalidOperationException("Channel darf nicht leer sein.");
        }

        await _subscriber.PublishAsync(RedisChannel.Literal(channel.Trim()), payload);
        AddMessage(channel.Trim(), payload, isCyclic);
    }

    public async Task StartCyclicPublishAsync(string channel, string payloadJson, int frequencyMs)
    {
        if (_subscriber == null)
        {
            throw new InvalidOperationException("Keine Redis-Verbindung vorhanden.");
        }

        if (frequencyMs < 10)
        {
            throw new InvalidOperationException("Frequenz muss mindestens 10 ms sein.");
        }

        await StopCyclicPublishAsync();

        var parsedPayload = ParseJsonObject(payloadJson);

        _cycleCts = new CancellationTokenSource();
        var token = _cycleCts.Token;

        _cycleTask = Task.Run(async () =>
        {
            OnCycleStatusChanged?.Invoke($"Zyklischer Versand gestartet ({frequencyMs} ms)");

            while (!token.IsCancellationRequested)
            {
                var cyclePayload = BuildCyclicPayload(parsedPayload);
                await PublishAsync(channel, cyclePayload, isCyclic: true);

                try
                {
                    await Task.Delay(frequencyMs, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }, token);
    }

    public async Task StopCyclicPublishAsync()
    {
        if (_cycleCts == null)
        {
            return;
        }

        _cycleCts.Cancel();

        if (_cycleTask != null)
        {
            try
            {
                await _cycleTask;
            }
            catch (TaskCanceledException)
            {
            }
        }

        _cycleTask = null;
        _cycleCts.Dispose();
        _cycleCts = null;

        OnCycleStatusChanged?.Invoke("Zyklischer Versand gestoppt");
    }

    public List<PublishedMessage> GetMessages()
    {
        return _messages.Reverse().ToList();
    }

    public void ClearMessages()
    {
        while (_messages.TryDequeue(out _))
        {
        }

        OnMessagesChanged?.Invoke();
    }

    private void AddMessage(string channel, string payload, bool isCyclic)
    {
        lock (_publishLock)
        {
            var entry = new PublishedMessage
            {
                Timestamp = DateTime.Now,
                Channel = channel,
                Payload = payload,
                FormattedPayload = FormatJson(payload),
                IsCyclic = isCyclic
            };

            _messages.Enqueue(entry);

            while (_messages.Count > MaxMessages)
            {
                _messages.TryDequeue(out _);
            }
        }

        OnMessagesChanged?.Invoke();
    }

    private static JsonObject ParseJsonObject(string payloadJson)
    {
        try
        {
            var node = JsonNode.Parse(payloadJson);
            if (node is not JsonObject jsonObject)
            {
                throw new InvalidOperationException("Für zyklischen Versand muss die Payload ein JSON-Objekt sein.");
            }

            return jsonObject;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ungültiges JSON: {ex.Message}");
        }
    }

    private string BuildCyclicPayload(JsonObject payloadTemplate)
    {
        var cloned = payloadTemplate.DeepClone().AsObject();
        cloned["TimeStamp"] = DateTimeOffset.UtcNow.ToString("O");
        return cloned.ToJsonString(_prettyJsonOptions);
    }

    private string FormatJson(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(doc, _prettyJsonOptions);
        }
        catch
        {
            return json;
        }
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}