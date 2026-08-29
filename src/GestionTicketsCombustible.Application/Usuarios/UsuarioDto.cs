namespace GestionTicketsCombustible.Application.Usuarios;

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string Roles { get; set; } = string.Empty;
}