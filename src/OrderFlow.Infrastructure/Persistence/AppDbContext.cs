using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common;
using OrderFlow.Domain.Entities; 


namespace OrderFlow.Infrastructure.Persistence ;
public class AppDbContext : DbContext , IAppDbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDashboardRow> DashboardRows => Set<OrderDashboardRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderDashboardRow>(entity =>
        {
            entity.HasKey(d => d.OrderId);
            entity.Property(d => d.OrderId)
                  .ValueGeneratedNever(); // OrderId mirrors an existing Order.Id; not DB-generated
            entity.Property(d => d.TotalPrice)
                  .HasPrecision(18, 2);
        });

        base.OnModelCreating(modelBuilder);
    }
}