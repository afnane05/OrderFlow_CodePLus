namespace OrderFlow.Domain.Entities ;

public class OrderItem{
    public int Id {get; set;}
    public decimal UnitPrice {get; set;}
    public int Quantity {get; set;}
    public string ProductName {get; set;}= string.Empty ; 

}