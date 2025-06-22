using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using OrdersService.Data;
using OrdersService.Messaging;
using OrdersService.Services;
using OrdersService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrdersDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(configuration["RabbitMq:Username"]);
            h.Password(configuration["RabbitMq:Password"]);
        });
    });
});

builder.Services.AddHostedService<OutboxPublisher>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
