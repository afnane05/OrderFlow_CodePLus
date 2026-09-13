using OrderFlow.Application.Common  ;
using OrderFlow.Domain.Entities ;
using MediatR ;

namespace OrderFlow.Application.Features.Orders.CreateOrder ;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IAppDbContext _context ; 
    public CreateOrderHandler(IAppDbContext context)
    {
        _context = context ;
    }
    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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
       //return the order id 
       return order.Id ;
    }
}