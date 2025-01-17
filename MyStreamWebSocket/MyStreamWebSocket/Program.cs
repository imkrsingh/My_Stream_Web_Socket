using Backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<FileStreamingService>();
builder.Services.AddSingleton<WebSocketHandler>();

var app = builder.Build();

// Enable WebSocket support
app.UseWebSockets();

// Middleware to handle WebSocket requests
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await webSocketHandler.HandleWebSocketAsync(context, webSocket);
    }
    else
    {
        await next();
    }
});

app.UseStaticFiles();

app.Run();
