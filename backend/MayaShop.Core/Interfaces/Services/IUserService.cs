using MayaShop.Core.DTOs.Users;

namespace MayaShop.Core.Interfaces.Services;

// Contrato del servicio de usuarios. Maneja la sincronización con Azure AD.
public interface IUserService
{
    // Busca o crea el usuario en la BD a partir del token de Azure AD.
    // Se llama automáticamente al primer login del usuario.
    Task<UserResponseDto> SyncUserFromTokenAsync(string azureOid, string email, string name);
    Task<UserResponseDto?> GetByIdAsync(int id);
}
