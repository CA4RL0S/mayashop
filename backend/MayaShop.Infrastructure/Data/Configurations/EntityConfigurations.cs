using MayaShop.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MayaShop.Infrastructure.Data.Configurations;

// Configuración Fluent API para la tabla users.
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.AzureOid).HasColumnName("azure_oid").IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.AzureOid).IsUnique();
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(256);
        builder.Property(u => u.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
        builder.Property(u => u.Role).HasColumnName("role").IsRequired().HasDefaultValue("customer").HasMaxLength(20);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
    }
}

// Configuración Fluent API para la tabla categories.
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
    }
}

// Configuración Fluent API para la tabla products.
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
        builder.Property(p => p.Description).HasColumnName("description").HasMaxLength(2000);
        // Precisión: 10 dígitos totales, 2 decimales (ej: 99999999.99 MXN)
        builder.Property(p => p.Price).HasColumnName("price").HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(p => p.Stock).HasColumnName("stock").IsRequired().HasDefaultValue(0);
        builder.Property(p => p.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
        builder.Property(p => p.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(p => p.CategoryId).HasColumnName("category_id");

        // Relación: Product pertenece a una Category
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// Configuración Fluent API para la tabla orders.
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");
        builder.Property(o => o.UserId).HasColumnName("user_id");
        builder.Property(o => o.Total).HasColumnName("total").HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(o => o.Status).HasColumnName("status").IsRequired().HasDefaultValue("pending").HasMaxLength(20);
        builder.Property(o => o.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

        // Relación: Order pertenece a un User
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// Configuración Fluent API para la tabla order_items.
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).HasColumnName("id");
        builder.Property(oi => oi.OrderId).HasColumnName("order_id");
        builder.Property(oi => oi.ProductId).HasColumnName("product_id");
        builder.Property(oi => oi.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(oi => oi.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(10,2)").IsRequired();

        // Relación: OrderItem pertenece a una Order (cascade delete)
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// Configuración Fluent API para la tabla carts.
public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.UserId).HasColumnName("user_id");

        builder.HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// Configuración Fluent API para la tabla cart_items.
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Id).HasColumnName("id");
        builder.Property(ci => ci.CartId).HasColumnName("cart_id");
        builder.Property(ci => ci.ProductId).HasColumnName("product_id");
        builder.Property(ci => ci.Quantity).HasColumnName("quantity").IsRequired().HasDefaultValue(1);

        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
