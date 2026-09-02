using System.ComponentModel.DataAnnotations;

namespace GestionTicketsCombustible.Application.Auth;

public class LoginDto
{
    [Required]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}