namespace RedisMonitor.Models;

public class RedisSettings
{
    public const int MinMessages = 10;
    public const int MaxMessagesLimit = 10000;
    
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string IncludeFilter { get; set; } = string.Empty;
    public string ExcludeFilter { get; set; } = string.Empty;
    
    private int _maxMessages = 1000;
    public int MaxMessages 
    { 
        get => _maxMessages;
        set => _maxMessages = Math.Clamp(value, MinMessages, MaxMessagesLimit);
    }
    
    public bool IsDarkMode { get; set; } = true;
    public bool GroupByChannel { get; set; } = false;
}
