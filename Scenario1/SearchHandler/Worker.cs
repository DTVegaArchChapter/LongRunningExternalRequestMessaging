namespace SearchHandler;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using System.Net.Http;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConnection _connection;

    private readonly IConfiguration _configuration;

    private static readonly HttpClient _httpClient = new();

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
                            var id = JsonSerializer.Deserialize<int>(Encoding.UTF8.GetString(e.Body.Span));

                            _logger.LogInformation($"q: {id}");

                            using var response = await _httpClient.GetAsync($"http://searchtargetapi1/search-product/{id}", stoppingToken);
                            response.EnsureSuccessStatusCode();
                            var responseBody = await response.Content.ReadAsStringAsync(stoppingToken);

                            _logger.LogInformation($"result: {responseBody}");

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