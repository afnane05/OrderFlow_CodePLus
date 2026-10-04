using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Common;

namespace OrderFlow.Application.Features.Orders.GetDashboard;

public class GetDashboardHandler : IRequestHandler<GetDashboardQuery, List<DashboardRowDto>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetDashboardHandler> _logger;

    public GetDashboardHandler(IAppDbContext context, ILogger<GetDashboardHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DashboardRowDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var rows = await _context.DashboardRows.ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {RowCount} dashboard rows", rows.Count);

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