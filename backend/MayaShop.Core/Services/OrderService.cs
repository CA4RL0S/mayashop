using MayaShop.Core.DTOs.Orders;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Core.Interfaces.Services;

namespace MayaShop.Core.Services;

// Servicio de órdenes. Maneja la creación de pedidos desde el carrito y la gestión de estados.
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;

    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(int userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int orderId)
    {
        var order = await _orderRepo.GetWithItemsAsync(orderId);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto)
    {
        var order = new Order { UserId = userId };
        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId)
                ?? throw new KeyNotFoundException($"Producto {item.ProductId} no encontrado");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para '{product.Name}'");

            // Capturar precio actual (snapshot histórico)
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);
            total += product.Price * item.Quantity;

            // Descontar del inventario
            product.Stock -= item.Quantity;
            await _productRepo.UpdateAsync(product);
        }

        order.Total = total;
        var created = await _orderRepo.AddAsync(order);
        var detailed = await _orderRepo.GetWithItemsAsync(created.Id);
        return MapToDto(detailed!);
    }

    public async Task<OrderResponseDto?> UpdateStatusAsync(int orderId, string status)
    {
        var order = await _orderRepo.GetWithItemsAsync(orderId);
        if (order is null) return null;

        order.Status = status;
        await _orderRepo.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepo.GetAllWithDetailsAsync();
        return orders.Select(MapToDto);
    }

    // Convierte entidad Order a DTO de respuesta
    private static OrderResponseDto MapToDto(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        UserName = o.User?.Name ?? string.Empty,
        Total = o.Total,
        Status = o.Status,
        CreatedAt = o.CreatedAt,
        Items = o.Items.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? string.Empty,
            ProductImageUrl = i.Product?.ImageUrl ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
}
