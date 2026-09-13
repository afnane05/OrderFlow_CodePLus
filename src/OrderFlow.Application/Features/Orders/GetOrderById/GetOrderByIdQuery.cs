using MediatR;

namespace OrderFlow.Application.Features.Orders.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderDetailsDto?>
{
    public int Id { get; set; }

    public GetOrderByIdQuery(int id)
    {
        Id = id;
    }
}