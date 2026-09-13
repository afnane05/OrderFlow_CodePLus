using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using System.Text.Json;

namespace OrderFlow.Application.Features.Orders.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(1);

    public GetOrderByIdHandler(IAppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"order:{request.Id}";

        var cached = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<OrderDetailsDto>(cached);
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
        {
            return null;
        }

        var dto = new OrderDetailsDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var serialized = JsonSerializer.Serialize(dto);
        await _cache.SetAsync(cacheKey, serialized, CacheExpiration, cancellationToken);

        return dto;
    }
}