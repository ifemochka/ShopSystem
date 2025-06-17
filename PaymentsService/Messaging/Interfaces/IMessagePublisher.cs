using PaymentsService.Models;

namespace PaymentsService.Messaging.Interfaces;
public interface IMessagePublisher
{
    Task EnqueueOutboxMessageAsync(OutboxMessage message);
}
