namespace MayaShop.Core.Entities;

// Representa un usuario del sistema. Puede ser cliente o administrador.
// El campo AzureOid es el Object ID del usuario en Azure AD (Microsoft Entra ID).
public class User
{
    public int Id { get; set; }

    // Object ID del usuario en Azure AD
    public string AzureOid { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Roles posibles: "customer" | "admin"
    public string Role { get; set; } = "customer";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación: pedidos del usuario
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
