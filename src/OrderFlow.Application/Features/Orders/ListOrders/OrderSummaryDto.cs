namespace OrderFlow.Application.Features.Orders.ListOrders ;
using OrderFlow.Domain.Enums ;
public class OrderSummaryDto
{
    public int Id {get; set;}
    
    public string CustomerName {get; set ;} = string.Empty ; 
    public decimal TotalPrice {get; set;}

    public int ItemCount {get; set;}
    public OrderStatus Status{get; set; } 
}