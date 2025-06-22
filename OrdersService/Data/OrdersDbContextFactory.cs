using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrdersService.Data;

public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ordersdb;Username=postgres;Password=postgres");

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
