using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Messaging;
using PaymentsService.Messaging.Interfaces;
using PaymentsService.Models;
using PaymentsService.Services.Interfaces;



namespace PaymentsService.Services;
public class PaymentsProcessor : IPaymentsProcessor
{
    private readonly PaymentsDbContext _db;
    private readonly IMessagePublisher _publisher;

    public PaymentsProcessor(PaymentsDbContext db, IMessagePublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<bool> ProcessPaymentAsync(Guid userId, decimal amount, Guid orderId)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null || account.Balance < amount)
        {
            await _publisher.EnqueueOutboxMessageAsync(new OutboxMessage
            {
                Type = "PaymentFailed",
                Payload = $"{{ \"orderId\": \"{orderId}\" }}"
            });

            await _db.SaveChangesAsync();
            return false;
        }

        account.Balance -= amount;
        _db.Accounts.Update(account);

        await _publisher.EnqueueOutboxMessageAsync(new OutboxMessage
        {
            Type = "PaymentSucceeded",
            Payload = $"{{ \"orderId\": \"{orderId}\" }}"
        });

        await _db.SaveChangesAsync();
        return true;
    }
}
