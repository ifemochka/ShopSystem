using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Messaging;

public class OutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IPublishEndpoint _publisher;

    public OutboxPublisher(IServiceScopeFactory scopeFactory, IPublishEndpoint publisher)
    {
        _scopeFactory = scopeFactory;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var messages = await db.OutboxMessages
                .Where(m => !m.Sent)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var msg in messages)
            {
                var type = Type.GetType($"OrdersService.Messaging.Events.{msg.Type}");
                if (type == null) continue;

                var obj = System.Text.Json.JsonSerializer.Deserialize(msg.Payload, type);
                if (obj == null) continue;

                await _publisher.Publish(obj, type, stoppingToken);
                msg.Sent = true;
            }

            await db.SaveChangesAsync();
            await Task.Delay(2000, stoppingToken); 
        }
    }
}
