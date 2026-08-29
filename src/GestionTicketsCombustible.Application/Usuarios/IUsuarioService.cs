namespace GestionTicketsCombustible.Application.Usuarios;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<EditarUsuarioDto?> ObtenerParaEditarAsync(int id);
    Task<(bool Exito, string[] Errores)> CrearAsync(CrearUsuarioDto dto);
    Task ActualizarAsync(EditarUsuarioDto dto);
    Task CambiarEstadoAsync(int id, bool activo);
    Task<List<string>> ObtenerRolesDisponiblesAsync();
}