namespace OrderFlow.Application.Features.Orders.CreateOrder ;
using MediatR ;

public class CreateOrderCommand : IRequest<int>
{
    public string CustomerName {get; set;} = string.Empty;

    public List<CreateOrderItem> Items{get; set;} = new() ;
}
public class CreateOrderItem
{
    public decimal UnitPrice {get; set;}
    public int Quantity {get; set;}
    public string ProductName {get; set;} = string.Empty ;
}