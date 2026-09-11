namespace OrderFlow.Domain.Entities ;
using OrderFlow.Domain.Enums;
using System.Linq ;

public class Order {
    public int Id {get; set;}
    public string CustomerName {get; set;} = string.Empty;

    private readonly List<OrderItem> _items = new() ;

    public IReadOnlyList<OrderItem> Items => _items ;

    public decimal TotalPrice => _items.Sum(i => i.UnitPrice * i.Quantity);
    public OrderStatus Status {get; set;} = OrderStatus.Pending;
    public DateTime CreatedAt {get; set;}

    public void AddItem(OrderItem item)
    {
        if(item == null) throw new ArgumentNullException(nameof(item));
        if(item.Quantity <= 0) throw new ArgumentException("Item quantity must be greater than zero",nameof(item));
        if(item.UnitPrice < 0) throw new ArgumentException("Item unit price shoud be greater than zero",nameof(item)) ;
        _items.Add(item);
    }
    public Order(string customerName)
    {
        if(string.IsNullOrWhiteSpace(customerName))throw new ArgumentException("Customer name is required",nameof(customerName));
        CustomerName = customerName ;
        Status = OrderStatus.Pending ;
        CreatedAt = DateTime.UtcNow ; 
    }
}   