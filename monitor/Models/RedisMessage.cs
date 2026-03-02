namespace RedisMonitor.Models;

public class RedisMessage
{
    public DateTime Timestamp { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string FormattedPayload { get; set; } = string.Empty;
}
