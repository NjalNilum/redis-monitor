using RedisMonitor.Components;
using RedisMonitor.Services;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Configure URLs with fallback to free port if configured port is in use
if (!builder.Environment.IsDevelopment())
{
    var configuredUrl = builder.Configuration["Urls"] ?? "http://localhost:5108";
    var uri = new Uri(configuredUrl.Split(';')[0]);
    
    if (!IsPortAvailable(uri.Port))
    {
        var freePort = GetAvailablePort(5108);
        var newUrl = $"{uri.Scheme}://{uri.Host}:{freePort}";
        builder.WebHost.UseUrls(newUrl);
        Console.WriteLine($"Port {uri.Port} is in use. Using port {freePort} instead.");
        Console.WriteLine($"Application URL: {newUrl}");
    }
    else
    {
        builder.WebHost.UseUrls(configuredUrl);
    }
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Redis Monitor Service as Singleton
builder.Services.AddSingleton<RedisMonitorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Helper methods for port availability
static bool IsPortAvailable(int port)
{
    try
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
        return true;
    }
    catch (SocketException)
    {
        return false;
    }
}

static int GetAvailablePort(int startPort = 5108)
{
    for (int port = startPort; port < startPort + 100; port++)
    {
        if (IsPortAvailable(port))
        {
            return port;
        }
    }
    // If no port found in range, let OS assign one
    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
    var endpoint = (IPEndPoint)socket.LocalEndPoint!;
    return endpoint.Port;
}
