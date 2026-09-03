namespace GestionTicketsCombustible.Application.Inventario;

public interface ITanqueService
{
    Task<IEnumerable<TanqueDto>> ObtenerTodosAsync();
    Task<TanqueDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearTanqueDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task DesactivarAsync(int id, int usuarioId, string nombreUsuario, string direccionIp);
    Task<IEnumerable<InventarioResumenDto>> ObtenerResumenInventarioAsync();
}