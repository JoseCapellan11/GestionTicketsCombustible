namespace GestionTicketsCombustible.Application.Departamentos;

public interface IDepartamentoService
{
    Task<List<DepartamentoDto>> ObtenerTodosAsync();
    Task<DepartamentoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(string nombre);
    Task ActualizarAsync(int id, string nombre);
    Task CambiarEstadoAsync(int id, bool activo);
}