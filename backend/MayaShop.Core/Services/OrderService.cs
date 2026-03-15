using MayaShop.Core.DTOs;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces;

namespace MayaShop.Core.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderFromCartAsync(int userId);
    Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(int userId);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task UpdateOrderStatusAsync(int id, string status);
}

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Cart> _cartRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly ICartService _cartService;

    public OrderService(
        IRepository<Order> orderRepo, 
        IRepository<Cart> cartRepo, 
        IRepository<Product> productRepo,
        ICartService cartService)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _cartService = cartService;
    }

    public async Task<OrderResponseDto> CreateOrderFromCartAsync(int userId)
    {
        // 1. Obtener carrito actual
        var cartDto = await _cartService.GetCartByUserIdAsync(userId);
        if (!cartDto.Items.Any())
            throw new Exception("El carrito está vacío, no se puede crear la orden.");

        // 2. Validar stock (una vez más, por seguridad antes de pagar)
        foreach (var item in cartDto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product == null || product.Stock < item.Quantity)
                throw new Exception($"Stock insuficiente para el producto {item.ProductName}");
        }

        // 3. Crear cabecera de la Orden
        var order = new Order
        {
            UserId = userId,
            Total = cartDto.Total,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        // 4. Mapear los items y reducir stock
        foreach (var item in cartDto.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });

            var product = await _productRepo.GetByIdAsync(item.ProductId);
            product!.Stock -= item.Quantity;
            await _productRepo.UpdateAsync(product);
        }

        await _orderRepo.AddAsync(order);
        await _orderRepo.SaveChangesAsync();

        // 5. Limpiar el carrito
        await _cartService.ClearCartAsync(userId);

        return await GetOrderByIdAsync(order.Id) ?? throw new Exception("Error al mapear la nueva orden.");
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _orderRepo.FindAsync(o => o.UserId == userId);
        var result = new List<OrderResponseDto>();
        
        foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
        {
            var orderDto = await GetOrderByIdAsync(order.Id);
            if (orderDto != null) result.Add(orderDto);
        }
        
        return result;
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return null;

        return new OrderResponseDto
        {
            Id = order.Id,
            Total = order.Total,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            // Items podrían ser cargados si configuramos el repositorio para Eager Loading.
            // Por simplicidad del genérico en esta fase los dejamos sin poblar en detalle de línea 
            // O se podrían cargar explícitamente usando inyección del dbcontext
        };
    }

    public async Task UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) throw new KeyNotFoundException("Orden no encontrada");

        order.Status = status; // Ej: Paid, Shipped, Delivered, Cancelled
        await _orderRepo.UpdateAsync(order);
        await _orderRepo.SaveChangesAsync();
    }
}
