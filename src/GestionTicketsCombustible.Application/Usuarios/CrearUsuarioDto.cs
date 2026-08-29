namespace GestionTicketsCombustible.Application.Usuarios;

public class CrearUsuarioDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}