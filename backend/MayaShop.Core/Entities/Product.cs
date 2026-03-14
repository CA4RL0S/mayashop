namespace MayaShop.Core.Entities;

// Producto en el catálogo de la tienda.
// Precio almacenado en MXN (pesos mexicanos).
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Precio en pesos mexicanos (MXN)
    public decimal Price { get; set; }

    // Unidades disponibles en inventario
    public int Stock { get; set; }

    // URL pública de la imagen del producto
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Llave foránea a Category
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navegación: líneas de pedidos que incluyen este producto
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
