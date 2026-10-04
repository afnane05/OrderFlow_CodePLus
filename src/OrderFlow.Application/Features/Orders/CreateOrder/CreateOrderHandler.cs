using OrderFlow.Application.Common  ;
using OrderFlow.Domain.Entities ;
using MediatR ;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Common;

namespace OrderFlow.Application.Features.Orders.CreateOrder ;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IAppDbContext _context ; 
    private readonly ILogger<CreateOrderHandler> _logger;
    public CreateOrderHandler(IAppDbContext context, ILogger<CreateOrderHandler> logger)
    {
        _context = context ;
        _logger = logger ;
    }
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var activity = OrderFlowActivitySource.Source.StartActivity("CreateOrder");
        activity?.SetTag("order.customer", request.CustomerName);

        try
        {
            //1- build the entity 
            var order = new Order(request.CustomerName);
            foreach (var item in request.Items)
            {
                var orderItem = new OrderItem{ 
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity

                };
                order.AddItem(orderItem);
            }

            //2- hande it to Ef core, not regestered in Db yet , just intending to insert it 
            _context.Orders.Add(order);
            //3-commit to database
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} created for {CustomerName}", order.Id, order.CustomerName);
            OrderFlowMetrics.OrdersCreated.Add(1);
            OrderFlowMetrics.PendingOrders.Add(1); // new orders start as Pending

            //return the order id 
            return order.Id ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for {CustomerName}", request.CustomerName);
            throw;
        }
    }
}