using OrderFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Domain.Entities;

public class OrderDashboardRow
{
    [Key]
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
}