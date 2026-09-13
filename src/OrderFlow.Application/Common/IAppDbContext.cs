using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities ;
namespace OrderFlow.Application.Common ;
public interface IAppDbContext
{
    DbSet<Order> Orders{ get ;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<OrderDashboardRow> DashboardRows { get; }
}
