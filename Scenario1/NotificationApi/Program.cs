using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(
    b =>
        {
            b.WithOrigins("http://localhost:8080")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });

app.MapHub<NotificationHub>("/notificationHub");
app.MapPost("/notify-clients/{requestId}", (HttpContext httpContext, string requestId, [FromBody]ProductInfo[] productInfo) =>
    {
        var hubContext = httpContext.RequestServices.GetRequiredService<IHubContext<NotificationHub>>();
        return hubContext.Clients.Group(requestId).SendAsync("ReceivePrice", productInfo);
    });
app.MapGet("/get-new-guid", () => Guid.NewGuid().ToString("N"));
app.Run();

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.GetHttpContext()!.Request.Query["request-id"]!);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.GetHttpContext()!.Request.Query["request-id"]!);
        await base.OnDisconnectedAsync(exception);
    }
}

internal record ProductInfo(string Store, string Name, decimal Price);