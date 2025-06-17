using Microsoft.EntityFrameworkCore;
using OrdersService.Messaging;
using OrdersService.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OrdersService.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
    }
}
