namespace GestionTicketsCombustible.Application.Usuarios;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<EditarUsuarioDto?> ObtenerParaEditarAsync(int id);
    Task<(bool Exito, string[] Errores)> CrearAsync(CrearUsuarioDto dto, int actorId, string actorNombreUsuario, string direccionIp);
    Task ActualizarAsync(EditarUsuarioDto dto, int actorId, string actorNombreUsuario, string direccionIp);
    Task CambiarEstadoAsync(int id, bool activo, int actorId, string actorNombreUsuario, string direccionIp);
    Task<List<string>> ObtenerRolesDisponiblesAsync();

    Task<(bool Exito, string[] Errores)> RestablecerPasswordAsync(int id, string nuevaPassword, int actorId, string actorNombreUsuario, string direccionIp);
}