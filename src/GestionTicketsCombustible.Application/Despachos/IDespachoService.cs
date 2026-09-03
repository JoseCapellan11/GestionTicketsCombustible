namespace GestionTicketsCombustible.Application.Despachos;

public interface IDespachoService
{
    Task<int> RegistrarAsync(CrearDespachoDto dto, int usuarioDespachadorId, string direccionIp);
    Task<DespachoDto?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<DespachoDto>> ObtenerTodosAsync();
}