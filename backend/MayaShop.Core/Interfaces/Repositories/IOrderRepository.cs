using MayaShop.Core.Entities;

namespace MayaShop.Core.Interfaces.Repositories;

// Repositorio específico para órdenes.
// Incluye queries para dashboard y gestión de pedidos.
public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetWithItemsAsync(int orderId);
    Task<IEnumerable<Order>> GetTodayOrdersAsync();
    Task<IEnumerable<Order>> GetAllWithDetailsAsync();
}
