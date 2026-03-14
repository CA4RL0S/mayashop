using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MayaShop.Infrastructure.Repositories;

// Repositorio de usuarios. Permite buscar por Azure OID (clave de identidad en Entra ID).
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByAzureOidAsync(string azureOid) =>
        await _context.Users.FirstOrDefaultAsync(u => u.AzureOid == azureOid);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
