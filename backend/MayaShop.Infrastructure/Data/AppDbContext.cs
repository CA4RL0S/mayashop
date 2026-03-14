using MayaShop.Core.Entities;
using MayaShop.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace MayaShop.Infrastructure.Data;

// DbContext principal de la aplicación. Define todos los DbSets y configuraciones de EF Core.
// Usa Fluent API para configurar relaciones, restricciones y nombres de columnas.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Conjuntos de entidades mapeadas a tablas PostgreSQL
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones individuales por entidad
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());

        // Datos semilla: categorías iniciales para Yucatán
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Artesanías" },
            new Category { Id = 2, Name = "Ropa y Textiles" },
            new Category { Id = 3, Name = "Alimentos y Bebidas" },
            new Category { Id = 4, Name = "Joyería" },
            new Category { Id = 5, Name = "Decoración" }
        );
    }
}
