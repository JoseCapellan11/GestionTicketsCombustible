using Microsoft.AspNetCore.Identity;

namespace GestionTicketsCombustible.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string NombreCompleto { get; set; } = string.Empty;
}