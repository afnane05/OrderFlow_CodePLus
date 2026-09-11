using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Entities; 


namespace OrderFlow.Infrastructure.Persistence ;
public class AppDbContext : DbContext , IAppDbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}
