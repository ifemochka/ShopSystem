using OrdersService.Data;
using OrdersService.DTOs;
using OrdersService.Messaging;
using OrdersService.Messaging.Events;
using OrdersService.Models;
using System.Text.Json;

namespace OrdersService.Services;

public class OrderService : Interfaces.IOrderService
{
    private readonly OrdersDbContext _context;

    public OrderService(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            UserId = request.UserId,
            Amount = request.Amount,
            Description = request.Description
        };

        await _context.Orders.AddAsync(order);

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Amount = order.Amount
        };

        var outbox = new OutboxMessage
        {
            Type = nameof(OrderCreatedEvent),
            Payload = JsonSerializer.Serialize(orderCreatedEvent)
        };

        await _context.OutboxMessages.AddAsync(outbox);
        await _context.SaveChangesAsync();

        return new OrderResponse { Id = order.Id, Status = order.Status };
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        return _context.Orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            Status = o.Status
        }).ToList();
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        return order == null ? null : new OrderResponse { Id = order.Id, Status = order.Status };
    }
}
