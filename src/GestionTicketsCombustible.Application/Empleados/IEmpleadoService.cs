namespace GestionTicketsCombustible.Application.Empleados;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();
    Task<EmpleadoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(EmpleadoDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task ActualizarAsync(EmpleadoDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task CambiarEstadoAsync(int id, bool activo, int usuarioId, string nombreUsuario, string direccionIp);
}