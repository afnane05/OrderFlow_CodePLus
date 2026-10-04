using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Application.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace OrderFlow.Infrastructure.BackgroundJobs;

public class DashboardRefreshWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DashboardRefreshWorker> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public DashboardRefreshWorker(IServiceScopeFactory scopeFactory, ILogger<DashboardRefreshWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Dashboard refresh started");
            var sw = Stopwatch.StartNew();

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                    await ProcessPendingOrdersAsync(context, stoppingToken);
                    await RefreshDashboardAsync(context, stoppingToken);
                }

                sw.Stop();
                OrderFlowMetrics.WorderRuns.Add(1);
                OrderFlowMetrics.WorkerDuration.Record(sw.Elapsed.TotalMilliseconds);
                _logger.LogInformation("Dashboard refresh completed in {ElapsedMs}ms", sw.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard refresh failed");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private static async Task ProcessPendingOrdersAsync(IAppDbContext context, CancellationToken cancellationToken)
    {
        var pendingOrders = await context.Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var order in pendingOrders)
        {
            order.Status = OrderStatus.Completed;
        }

        if (pendingOrders.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            OrderFlowMetrics.PendingOrders.Add(-pendingOrders.Count);
        }
    }

    private static async Task RefreshDashboardAsync(IAppDbContext context, CancellationToken cancellationToken)
    {
        var orders = await context.Orders.ToListAsync(cancellationToken);

        var existingRows = await context.DashboardRows.ToListAsync(cancellationToken);
        foreach (var row in existingRows)
        {
            context.DashboardRows.Remove(row);
        }

        foreach (var order in orders)
        {
            context.DashboardRows.Add(new OrderDashboardRow
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                ItemCount = order.Items.Count,
                TotalPrice = order.TotalPrice,
                Status = order.Status
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}