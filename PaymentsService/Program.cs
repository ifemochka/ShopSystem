using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;
using OrdersService.Messaging.Events;
using PaymentsService.Data;
using PaymentsService.Messaging;
using PaymentsService.Messaging.Interfaces;
using PaymentsService.Services;
using PaymentsService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PaymentsDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPaymentsProcessor, PaymentsProcessor>();
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(ctx);
        });
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
