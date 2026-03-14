using MayaShop.Core.DTOs.Products;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Core.Interfaces.Services;

namespace MayaShop.Core.Services;

// Servicio de productos. Aplica lógica de negocio y convierte entidades a DTOs.
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(int? categoryId, string? search)
    {
        IEnumerable<Product> products;

        if (!string.IsNullOrWhiteSpace(search))
            products = await _productRepo.SearchAsync(search);
        else if (categoryId.HasValue)
            products = await _productRepo.GetByCategoryAsync(categoryId.Value);
        else
            products = await _productRepo.GetActiveProductsAsync();

        return products.Select(MapToDto);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };

        var created = await _productRepo.AddAsync(product);
        return MapToDto(created);
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product is null) return null;

        // Actualizar solo los campos proporcionados (null = sin cambio)
        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        if (dto.ImageUrl is not null) product.ImageUrl = dto.ImageUrl;
        if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

        await _productRepo.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product is null) return false;

        // Soft delete: marcar como inactivo en lugar de eliminar físicamente
        product.IsActive = false;
        await _productRepo.UpdateAsync(product);
        return true;
    }

    // Convierte entidad Product a DTO de respuesta
    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        ImageUrl = p.ImageUrl,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty
    };
}
