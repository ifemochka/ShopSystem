namespace PaymentsService.Services.Interfaces;
public interface IPaymentsProcessor
{
    Task<bool> ProcessPaymentAsync(Guid userId, decimal amount, Guid orderId);
}
