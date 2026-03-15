using MayaShop.Core.DTOs;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces;

namespace MayaShop.Core.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeInactive = false);
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(ProductCreateRequestDto dto);
    Task UpdateProductAsync(int id, ProductCreateRequestDto dto);
    Task DeleteProductAsync(int id); // Ejecutará un Soft Delete
}

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<Category> _categoryRepo;

    public ProductService(IRepository<Product> productRepo, IRepository<Category> categoryRepo)
    {
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(bool includeInactive = false)
    {
        var products = includeInactive 
            ? await _productRepo.GetAllAsync() 
            : await _productRepo.FindAsync(p => p.IsActive);
        
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepo.FindAsync(p => p.CategoryId == categoryId && p.IsActive);
        return products.Select(MapToDto);
    }

    public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) return null;
        
        // Dado que usamos el repo genérico que quizás no incluya categorías de forma greedy (Eager loading),
        // Aquí podríamos mapear la categoría manualmente si es necesario o configurar un repositorio especializado.
        // Por ahora lo mapearemos de forma segura asumiendo que el Navigation Property se maneja a nivel query si se requiriera.
        var category = await _categoryRepo.GetByIdAsync(product.CategoryId);
        if (product.Category == null && category != null) 
            product.Category = category;

        return MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateProductAsync(ProductCreateRequestDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            IsActive = true
        };

        await _productRepo.AddAsync(product);
        await _productRepo.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task UpdateProductAsync(int id, ProductCreateRequestDto dto)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;

        await _productRepo.UpdateAsync(product);
        await _productRepo.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

        // Soft delete: no borramos el producto, lo desactivamos
        product.IsActive = false;
        await _productRepo.UpdateAsync(product);
        await _productRepo.SaveChangesAsync();
    }

    private ProductResponseDto MapToDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            CategoryName = product.Category?.Name ?? "Unknown Category", // Manejo nulo temporal si no hay eager loading
            IsActive = product.IsActive
        };
    }
}
