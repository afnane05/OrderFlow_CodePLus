using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Text.Json.Serialization;
using OrderFlow.Application.Common;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Caching;
using OrderFlow.Infrastructure.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers + JSON config ---
// Serializes enums (OrderStatus) as their string name ("Pending") instead
// of the underlying int (0) in API responses.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Database (SQL Server via EF Core) ---
// Connection string comes from appsettings.json / appsettings.Development.json
// under a "DefaultConnection" key, per the NFR requiring config to come
// from appsettings/environment variables.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AppDbContext is registered above as itself (needed by EF Core tooling/migrations).
// This line additionally registers it as the IAppDbContext interface, so anywhere
// IAppDbContext is requested (handlers, the background worker), EF Core's DI
// container hands back the same AppDbContext instance.
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// --- Redis cache ---
// Connection string also comes from configuration, same convention as SQL Server.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// --- MediatR ---
// Scans the assembly containing CreateOrderCommand for every IRequestHandler
// implementation and registers them all automatically — you don't manually
// register each handler one by one.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

// --- Background worker ---
// Registered as a hosted service: ASP.NET Core starts it automatically when
// the app starts, and stops it when the app shuts down.
builder.Services.AddHostedService<DashboardRefreshWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();