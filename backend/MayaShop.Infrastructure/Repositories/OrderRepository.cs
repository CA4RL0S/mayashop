using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MayaShop.Infrastructure.Repositories;

// Repositorio de órdenes. Incluye eager loading de items y usuarios.
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId) =>
        await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<Order?> GetWithItemsAsync(int orderId) =>
        await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task<IEnumerable<Order>> GetTodayOrdersAsync()
    {
        // Filtrar pedidos del día actual (UTC)
        var todayUtc = DateTime.UtcNow.Date;
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CreatedAt.Date == todayUtc)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllWithDetailsAsync() =>
        await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
}
