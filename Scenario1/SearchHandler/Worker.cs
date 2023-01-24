namespace SearchHandler;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConnection _connection;

    private readonly IConfiguration _configuration;

    private static readonly HttpClient _httpClient = new();

    private sealed record ProductInfo(string Store, string Name, decimal Price);
    private sealed record SearchResult(string ClientId, DateTime StartTime, DateTime EndTime, int DurationInMs, ProductInfo[] Products);
    private sealed record SearchInfo(int ProductId, string ClientId, DateTime StartTime);

    public Worker(ILogger<Worker> logger, IConnection connection, IConfiguration configuration)
    {
        _logger = logger;
        _connection = connection;
        _configuration = configuration;
        _httpClient.Timeout = TimeSpan.FromMilliseconds(configuration.GetValue("HttpClientTimeout", 60_000));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumeTask = Task.Factory.StartNew(() =>
            {
                var model = _connection.CreateModel();

                model.QueueDeclare("Search", true, false, false);

                var consumer = new EventingBasicConsumer(model);
                consumer.Received += async (_, e) =>
                    {
                        try
                        {
                            var searchInfo = JsonSerializer.Deserialize<SearchInfo>(Encoding.UTF8.GetString(e.Body.Span));

                            using var response = await _httpClient.GetAsync($"http://searchtargetapi1/search-product/{searchInfo!.ProductId}", stoppingToken);
                            response.EnsureSuccessStatusCode();
                            var responseBody = await response.Content.ReadFromJsonAsync<ProductInfo>(cancellationToken: stoppingToken);

                            var endTime = DateTime.Now;
                            using var notificationResponse = await _httpClient.PostAsJsonAsync($"http://notificationapi/notify-clients", new SearchResult(searchInfo.ClientId, searchInfo.StartTime, endTime, (int)(endTime - searchInfo.StartTime).TotalMilliseconds, new[] { responseBody! }), stoppingToken);
                            notificationResponse.EnsureSuccessStatusCode();

                            model.BasicAck(e.DeliveryTag, false);
                        }
                        catch (Exception)
                        {
                            model.BasicNack(e.DeliveryTag, false, true);

                            throw;
                        }
                    };

                model.BasicQos(0, _configuration.GetValue("PrefetchCount", (ushort)10), false);
                return (model ,model.BasicConsume("Search", false, consumer));
            }, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);

        var (channel, consumeTag) = await consumeTask;
        if (channel != null)
        {
            if (consumeTag != null)
            {
                channel.BasicCancel(consumeTag);
            }

            channel.Close(200, "Goodbye");
            channel.Dispose();
        }
    }
}