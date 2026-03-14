using MayaShop.Core.DTOs.Orders;

namespace MayaShop.Core.Interfaces.Services;

// Contrato del servicio de órdenes. Gestiona el ciclo de vida de los pedidos.
public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(int userId);
    Task<OrderResponseDto?> GetByIdAsync(int orderId);
    Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto);
    Task<OrderResponseDto?> UpdateStatusAsync(int orderId, string status);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
}
