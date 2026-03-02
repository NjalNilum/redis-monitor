using StackExchange.Redis;

namespace RedisSimulator.Models;

public class RedisSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6380;
    public bool IsDarkMode { get; set; } = true;

    public void ApplyConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        ConfigurationOptions options;
        try
        {
            options = ConfigurationOptions.Parse(connectionString);
        }
        catch
        {
            return;
        }

        if (options.EndPoints.Count == 0)
        {
            return;
        }

        if (options.EndPoints[0] is System.Net.DnsEndPoint dnsEndPoint)
        {
            Host = dnsEndPoint.Host;
            Port = dnsEndPoint.Port;
            return;
        }

        if (options.EndPoints[0] is System.Net.IPEndPoint ipEndPoint)
        {
            Host = ipEndPoint.Address.ToString();
            Port = ipEndPoint.Port;
        }
    }
}