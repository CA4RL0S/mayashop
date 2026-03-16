using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces;

namespace MayaShop.Core.Services;

public interface IUserService
{
    Task<User> SyncUserAsync(string azureOid, string email, string name);
    Task<User?> GetUserByAzureOidAsync(string azureOid);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepo;

    public UserService(IRepository<User> userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<User?> GetUserByAzureOidAsync(string azureOid)
    {
        return await _userRepo.FirstOrDefaultAsync(u => u.AzureOid == azureOid);
    }

    public async Task<User> SyncUserAsync(string azureOid, string email, string name)
    {
        var existingUser = await GetUserByAzureOidAsync(azureOid);
        if (existingUser != null)
        {
            // Opcional: actualizar nombre/email si cambiaron en Azure AD
            return existingUser;
        }

        // Es un usuario nuevo
        var newUser = new User
        {
            AzureOid = azureOid,
            Email = email ?? "no-email@example.com",
            Name = name ?? "Usuario Anónimo",
            Role = "customer" // Rol por defecto
        };

        await _userRepo.AddAsync(newUser);
        await _userRepo.SaveChangesAsync();

        return newUser;
    }
}
