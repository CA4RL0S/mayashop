namespace MayaShop.Core.DTOs;

public class OrderResponseDto
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// Para crear una orden, el usuario solo necesita invocar un endpoint, y tomamos su carrito activo
public class OrderCreateRequestDto
{
    // Vacío por el momento: la lógica se basa en el carrito actual del usuario autenticado
}
