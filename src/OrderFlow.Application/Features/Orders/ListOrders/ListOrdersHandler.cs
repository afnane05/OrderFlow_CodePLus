using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;

namespace OrderFlow.Application.Features.Orders.ListOrders;

public class ListOrdersHandler : IRequestHandler<ListOrdersQuery, List<OrderSummaryDto>>
{
    private readonly IAppDbContext _context;

    public ListOrdersHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderSummaryDto>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders.ToListAsync(cancellationToken);

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