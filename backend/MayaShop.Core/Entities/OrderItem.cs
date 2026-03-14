namespace MayaShop.Core.Entities;

// Línea de un pedido. Representa un producto específico dentro de una orden.
// El precio unitario se guarda al momento de la compra para mantener histórico.
public class OrderItem
{
    public int Id { get; set; }

    // Llave foránea al pedido padre
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    // Llave foránea al producto comprado
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // Cantidad de unidades compradas
    public int Quantity { get; set; }

    // Precio unitario al momento de la compra (snapshot del precio)
    public decimal UnitPrice { get; set; }
}
