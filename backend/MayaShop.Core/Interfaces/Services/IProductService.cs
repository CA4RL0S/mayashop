using MayaShop.Core.DTOs.Products;

namespace MayaShop.Core.Interfaces.Services;

// Contrato del servicio de productos. Contiene la lógica de negocio de catálogo.
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync(int? categoryId, string? search);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);
}
