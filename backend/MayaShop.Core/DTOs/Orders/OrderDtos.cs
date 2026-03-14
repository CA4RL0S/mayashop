namespace MayaShop.Core.DTOs.Orders;

// DTO de respuesta para una orden completa (con sus items).
public class OrderResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

// DTO de respuesta para una línea de orden.
public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

// DTO para crear una orden desde el carrito del cliente.
public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

// DTO para cada producto dentro de una orden nueva.
public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

// DTO para cambiar el estado de una orden (solo admin).
public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}
