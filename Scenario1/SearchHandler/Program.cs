using RabbitMQ.Client;

using SearchHandler;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton(
            _ =>
                {
                    var factory = new ConnectionFactory { HostName = "rabbitmq", UserName = "guest", Password = "guest" };
                    return factory.CreateConnection();
                });
    })
    .Build();

await host.RunAsync();
