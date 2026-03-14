using MayaShop.Core.DTOs.Users;
using MayaShop.Core.Entities;
using MayaShop.Core.Interfaces.Repositories;
using MayaShop.Core.Interfaces.Services;

namespace MayaShop.Core.Services;

// Servicio de usuarios. Sincroniza el perfil desde Azure AD a la base de datos local.
public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // Busca el usuario por Azure OID. Si no existe, lo crea automáticamente (primer login).
    public async Task<UserResponseDto> SyncUserFromTokenAsync(string azureOid, string email, string name)
    {
        var existingUser = await _userRepo.GetByAzureOidAsync(azureOid);

        if (existingUser is not null)
            return MapToDto(existingUser);

        // Primer login: crear el usuario en la base de datos
        var newUser = new User
        {
            AzureOid = azureOid,
            Email = email,
            Name = name,
            Role = "customer"
        };

        var created = await _userRepo.AddAsync(newUser);
        return MapToDto(created);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    // Convierte entidad User a DTO de respuesta
    private static UserResponseDto MapToDto(User u) => new()
    {
        Id = u.Id,
        AzureOid = u.AzureOid,
        Email = u.Email,
        Name = u.Name,
        Role = u.Role
    };
}
