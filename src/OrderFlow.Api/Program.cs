using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Text.Json.Serialization;
using OrderFlow.Application.Common;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Caching;
using OrderFlow.Infrastructure.BackgroundJobs;


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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app mapping to health check 
app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();