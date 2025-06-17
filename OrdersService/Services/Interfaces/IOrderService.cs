using OrdersService.DTOs;

namespace OrdersService.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();
    Task<OrderResponse?> GetOrderByIdAsync(Guid id);
}
