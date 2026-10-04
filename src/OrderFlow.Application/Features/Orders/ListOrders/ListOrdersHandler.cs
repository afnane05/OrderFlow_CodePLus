using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Common;
namespace OrderFlow.Application.Features.Orders.ListOrders;

public class ListOrdersHandler : IRequestHandler<ListOrdersQuery, List<OrderSummaryDto>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<ListOrdersHandler> _logger;

    public ListOrdersHandler(IAppDbContext context, ILogger<ListOrdersHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<OrderSummaryDto>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        using var activity = OrderFlowActivitySource.Source.StartActivity("ListOrders");

        var orders = await _context.Orders.ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {OrderCount} orders", orders.Count);

        return orders.Select(o => new OrderSummaryDto
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            TotalPrice = o.TotalPrice,
            ItemCount = o.Items.Count,
            Status = o.Status
        }).ToList();
    }
}