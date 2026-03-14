namespace MayaShop.Core.Entities;

// Pedido realizado por un usuario.
// Estados posibles: pending | confirmed | shipped | delivered | cancelled.
public class Order
{
    public int Id { get; set; }

    // Llave foránea al usuario que realizó el pedido
    public int UserId { get; set; }
    public User? User { get; set; }

    // Total del pedido en MXN
    public decimal Total { get; set; }

    // Estado del pedido
    public string Status { get; set; } = "pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación: líneas del pedido (productos individuales)
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
