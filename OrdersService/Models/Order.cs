namespace OrdersService.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "NEW"; // NEW, FINISHED, CANCELLED
}
