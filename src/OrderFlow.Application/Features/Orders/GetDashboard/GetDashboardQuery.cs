using MediatR;

namespace OrderFlow.Application.Features.Orders.GetDashboard;

public class GetDashboardQuery : IRequest<List<DashboardRowDto>>
{
}