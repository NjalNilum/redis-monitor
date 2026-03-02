namespace RedisSimulator.Models;

public class PublishedMessage
{
    public DateTime Timestamp { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string FormattedPayload { get; set; } = string.Empty;
    public bool IsCyclic { get; set; }
}