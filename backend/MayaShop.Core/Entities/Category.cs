namespace MayaShop.Core.Entities;

// Categoría de productos. Ejemplo: Artesanías, Ropa, Alimentos, etc.
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navegación: productos en esta categoría
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
