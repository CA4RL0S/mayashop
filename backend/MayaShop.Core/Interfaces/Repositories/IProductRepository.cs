using MayaShop.Core.Entities;

namespace MayaShop.Core.Interfaces.Repositories;

// Repositorio específico para productos con queries adicionales.
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchAsync(string term);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
}
