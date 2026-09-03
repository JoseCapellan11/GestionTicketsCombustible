namespace GestionTicketsCombustible.Application.Vehiculos;

public interface IVehiculoService
{
    Task<List<VehiculoDto>> ObtenerTodosAsync();
    Task<VehiculoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(VehiculoDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task ActualizarAsync(VehiculoDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task CambiarEstadoAsync(int id, bool activo, int usuarioId, string nombreUsuario, string direccionIp);
}