namespace OrderFlow.Application.Features.Orders.GetDashboard ;
using OrderFlow.Domain.Enums ;

public class DashboardRowDto
{
    public int Id {get; set;}
    public string CustomerName {get; set;} = string.Empty;

    public int ItemCount {get; set;}

    public decimal TotalPrice {get; set;}
    public OrderStatus Status {get; set;} 
    
}