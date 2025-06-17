using PaymentsService.Data;
using PaymentsService.Messaging.Interfaces;
using PaymentsService.Models;

namespace PaymentsService.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly PaymentsDbContext _db;

    public MessagePublisher(PaymentsDbContext db)
    {
        _db = db;
    }

    public async Task EnqueueOutboxMessageAsync(OutboxMessage message)
    {
        await _db.OutboxMessages.AddAsync(message);
    }
}
