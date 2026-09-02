namespace GestionTicketsCombustible.Application.Inventario;

public interface ITanqueService
{
    Task<IEnumerable<TanqueDto>> ObtenerTodosAsync();
    Task<TanqueDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearTanqueDto dto);
    Task DesactivarAsync(int id);
    Task<IEnumerable<InventarioResumenDto>> ObtenerResumenInventarioAsync();
}