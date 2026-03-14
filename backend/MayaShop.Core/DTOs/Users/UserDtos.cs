namespace MayaShop.Core.DTOs.Users;

/// <summary>
/// DTO de respuesta para un usuario. Se devuelve tras el sync con Azure AD.
/// </summary>
public class UserResponseDto
{
    public int Id { get; set; }
    public string AzureOid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
