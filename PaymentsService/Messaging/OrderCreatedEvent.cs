namespace OrdersService.Messaging.Events;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Messaging.Interfaces;
using PaymentsService.Models;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly PaymentsDbContext _db;

    public OrderCreatedConsumer(PaymentsDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var evt = context.Message;

        // идемпотентность через Inbox
        if (await _db.InboxMessages.AnyAsync(x => x.Id == context.MessageId)) return;

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == evt.UserId);
        if (account == null || account.Balance < evt.Amount)
        {
            // лог ошибки
            return;
        }

        account.Balance -= evt.Amount;

        _db.InboxMessages.Add(new InboxMessage
        {
            Id = context.MessageId ?? Guid.NewGuid(),
            Payload = System.Text.Json.JsonSerializer.Serialize(evt),
            ReceivedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
