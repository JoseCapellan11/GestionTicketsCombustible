namespace GestionTicketsCombustible.Application.Vehiculos;

public interface IVehiculoService
{
    Task<List<VehiculoDto>> ObtenerTodosAsync();
    Task<VehiculoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(VehiculoDto dto);
    Task ActualizarAsync(VehiculoDto dto);
    Task CambiarEstadoAsync(int id, bool activo);
}