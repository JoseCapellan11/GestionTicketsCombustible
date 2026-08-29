namespace GestionTicketsCombustible.Application.Usuarios;

public class EditarUsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Rol { get; set; } = string.Empty;
}