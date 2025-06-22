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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

    const int maxRetries = 10;
    int retry = 0;
    while (true)
    {
        try
        {
            db.Database.Migrate(); 
            break; 
        }
        catch (Exception ex)
        {
            retry++;
            if (retry > maxRetries)
                throw;

            Console.WriteLine($"[Database] Retry {retry} - waiting 5s... {ex.Message}");
            Thread.Sleep(5000); 
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
