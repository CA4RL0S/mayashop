using MayaShop.Core.Entities;

namespace MayaShop.Core.Interfaces.Repositories;

// Repositorio para usuarios. Permite buscar por Azure OID para sincronizar el perfil.
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByAzureOidAsync(string azureOid);
    Task<User?> GetByEmailAsync(string email);
}
