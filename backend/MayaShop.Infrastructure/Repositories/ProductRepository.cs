using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MayaShop.Infrastructure.Repositories;

// Repositorio de productos con queries específicos de búsqueda y filtrado.
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) =>
        await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string term) =>
        await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive &&
                (p.Name.ToLower().Contains(term.ToLower()) ||
                 p.Description.ToLower().Contains(term.ToLower())))
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetActiveProductsAsync() =>
        await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
}
