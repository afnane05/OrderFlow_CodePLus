using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrderFlow.Infrastructure.BackgroundJobs;

public class DashboardRefreshWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public DashboardRefreshWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                await ProcessPendingOrdersAsync(context, stoppingToken);
                await RefreshDashboardAsync(context, stoppingToken);
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