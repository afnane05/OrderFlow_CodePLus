namespace OrderFlow.Application.Features.Orders.GetOrderById ;
using OrderFlow.Domain.Entities ;
using OrderFlow.Domain.Enums ;

public class OrderDetailsDto
{
    public int Id {get; set;}
    public string CustomerName {get; set;} = string.Empty;

    public decimal TotalPrice;
    public OrderStatus Status {get; set;} 
    public DateTime CreatedAt {get; set;}
    public List<OrderItemDto> Items{get; set;} = new() ;
}
public class OrderItemDto
{
    public decimal UnitPrice {get; set;}
    public int Quantity {get; set;}
    public string ProductName {get; set;} = string.Empty ;
}