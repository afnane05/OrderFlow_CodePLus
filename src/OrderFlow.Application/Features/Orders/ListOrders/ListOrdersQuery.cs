using MediatR;

namespace OrderFlow.Application.Features.Orders.ListOrders;

public class ListOrdersQuery : IRequest<List<OrderSummaryDto>>
{
}