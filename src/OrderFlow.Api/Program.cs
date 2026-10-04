using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Text.Json.Serialization;
using OrderFlow.Application.Common;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Caching;
using OrderFlow.Infrastructure.BackgroundJobs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

builder.Services.AddHostedService<DashboardRefreshWorker>();

//health checks 
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), name: "SQL Server")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!, name: "Redis");

//Metrics
builder.Services.AddOpenTelemetry()
    .WithMetrics(m=> m 
        .AddAspNetCoreInstrumentation()
        .AddMeter("OrderFlow")
        .AddPrometheusExporter());

//Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation()
        .AddSource("OrderFlow")
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4317");
        }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app mapping to health check 
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

//Mapping Metrics
app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();