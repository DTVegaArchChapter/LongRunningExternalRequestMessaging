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
app.MapPost("/notify-clients", (HttpContext httpContext, [FromBody]ProductInfo[] productInfo) =>
    {
        var hubContext = httpContext.RequestServices.GetRequiredService<IHubContext<NotificationHub>>();
        return hubContext.Clients.All.SendAsync("ReceivePrice", productInfo);
    });
app.Run();

public class NotificationHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

internal record ProductInfo(string Store, string Name, decimal Price);