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
app.MapPost("/notify-clients", (HttpContext httpContext, [FromBody]SearchResult searchResult) =>
    {
        var hubContext = httpContext.RequestServices.GetRequiredService<IHubContext<NotificationHub>>();
        return hubContext.Clients.Group(searchResult.ClientId).SendAsync("ReceivePrice", searchResult);
    });
app.MapGet("/get-new-guid", () => Guid.NewGuid().ToString("N"));
app.Run();

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, Context.GetHttpContext()!.Request.Query["client-id"]!);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.GetHttpContext()!.Request.Query["client-id"]!);
        await base.OnDisconnectedAsync(exception);
    }
}

internal sealed record ProductInfo(string Store, string Name, decimal Price);
internal sealed record SearchResult(string ClientId, DateTime StartTime, DateTime EndTime, int DurationInMs, ProductInfo[] Products);