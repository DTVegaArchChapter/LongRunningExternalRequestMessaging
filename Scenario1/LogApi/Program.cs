using Microsoft.AspNetCore.Mvc;

using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IElasticClient>(_ => new ElasticClient(new ConnectionSettings(new Uri("http://elasticsearch:9200"))
    .BasicAuthentication("elastic", "Password1")
    .DefaultIndex("request")
    .DefaultMappingFor<RequestInfo>(x => x.IdProperty(y => y.RequestId).IndexName("request"))));
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
            elasticClient.IndexDocumentAsync(requestInfo);
            return Results.Ok();
        });

app.Run();

internal sealed record RequestInfo(string RequestId, DateTime StartTime, DateTime EndTime, int DurationInMs);