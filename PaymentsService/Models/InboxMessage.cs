namespace PaymentsService.Models;
public class InboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
