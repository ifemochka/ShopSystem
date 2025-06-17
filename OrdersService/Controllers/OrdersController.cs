using Microsoft.AspNetCore.Mvc;
using OrdersService.DTOs;
using OrdersService.Services.Interfaces;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {

        var result = await _service.CreateOrderAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _service.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _service.GetOrderByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }


}
