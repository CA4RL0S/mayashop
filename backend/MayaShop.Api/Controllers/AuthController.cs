using System.Security.Claims;
using MayaShop.Core.Entities;
using MayaShop.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MayaShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Solo usuarios con Token de Azure AD válido
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("sync")]
    public async Task<ActionResult<User>> SyncUser()
    {
        // 1. Extraemos los claims emitidos por Microsoft Entra ID
        var oidClaim = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier") 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        var nameClaim = User.FindFirst("name") ?? User.FindFirst(ClaimTypes.Name);
        var emailClaim = User.FindFirst("preferred_username") ?? User.FindFirst(ClaimTypes.Email);

        if (oidClaim == null)
            return Unauthorized(new { message = "El token no contiene un ObjectIdentifier válido desde Azure AD." });

        var oid = oidClaim.Value;
        var name = nameClaim?.Value ?? "Usuario Anónimo";
        var email = emailClaim?.Value ?? "no-email";

        // 2. Sincronizamos en nuestra Base de Datos PostgreSQL
        var user = await _userService.SyncUserAsync(oid, email, name);

        // Retornamos la info para que el frontend React sepa su ID interno (int) y su Rol ("customer"|"admin")
        return Ok(new { user.Id, user.Email, user.Name, user.Role });
    }
}
