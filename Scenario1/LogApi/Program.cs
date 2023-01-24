using Microsoft.AspNetCore.Mvc;

using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IElasticClient>(_ => new ElasticClient(new ConnectionSettings(new Uri("http://elasticsearch:9200"))
    .DefaultIndex("request")
    .DefaultMappingFor<RequestDocument>(x => x.IdProperty(y => y.Id).IndexName("request"))));
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(
    b =>
        {
            b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });

// Configure the HTTP request pipeline.
app.MapPost(
    "Add",
    (HttpContext httpContext, [FromBody]RequestInfo requestInfo) =>
        {
            var elasticClient = httpContext.RequestServices.GetRequiredService<IElasticClient>();
            elasticClient.IndexDocumentAsync(new RequestDocument(Guid.NewGuid().ToString("N"), requestInfo.ClientId, requestInfo.StartTime, requestInfo.EndTime, requestInfo.DurationInMs));
            return Results.Ok();
        });

app.Run();

internal sealed record RequestDocument(string Id, string ClientId, DateTime StartTime, DateTime EndTime, int DurationInMs);
internal sealed record RequestInfo(string ClientId, DateTime StartTime, DateTime EndTime, int DurationInMs);