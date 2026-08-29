namespace GestionTicketsCombustible.Application.Empleados;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> ObtenerTodosAsync();
    Task<EmpleadoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(EmpleadoDto dto);
    Task ActualizarAsync(EmpleadoDto dto);
    Task CambiarEstadoAsync(int id, bool activo);
}