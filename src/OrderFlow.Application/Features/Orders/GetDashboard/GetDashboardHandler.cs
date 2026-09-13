using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;

namespace OrderFlow.Application.Features.Orders.GetDashboard;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, List<DashboardRowDto>>
{
    private readonly IAppDbContext _context;

    public GetDashboardHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DashboardRowDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var rows = await _context.DashboardRows.ToListAsync(cancellationToken);

        return rows.Select(r => new DashboardRowDto
        {
            Id = r.OrderId,
            CustomerName = r.CustomerName,
            ItemCount = r.ItemCount,
            TotalPrice = r.TotalPrice,
            Status = r.Status
        }).ToList();
    }
}