using RedisSimulator.Components;
using RedisSimulator.Services;
using System.Net;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var configuredUrl = builder.Configuration["Urls"] ?? "http://localhost:5110";
    var uri = new Uri(configuredUrl.Split(';')[0]);

    if (!IsPortAvailable(uri.Port))
    {
        var freePort = GetAvailablePort(5110);
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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<RedisPublisherService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

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

static int GetAvailablePort(int startPort = 5110)
{
    for (int port = startPort; port < startPort + 100; port++)
    {
        if (IsPortAvailable(port))
        {
            return port;
        }
    }

    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
    var endpoint = (IPEndPoint)socket.LocalEndPoint!;
    return endpoint.Port;
}