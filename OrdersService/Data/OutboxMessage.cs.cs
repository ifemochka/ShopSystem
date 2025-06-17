namespace OrdersService.Messaging;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public bool Sent { get; set; } = false;
}
