namespace GestionTicketsCombustible.Application.Departamentos;

public interface IDepartamentoService
{
    Task<List<DepartamentoDto>> ObtenerTodosAsync();
    Task<DepartamentoDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(string nombre, int usuarioId, string nombreUsuario, string direccionIp);
    Task ActualizarAsync(int id, string nombre, int usuarioId, string nombreUsuario, string direccionIp);
    Task CambiarEstadoAsync(int id, bool activo, int usuarioId, string nombreUsuario, string direccionIp);
}