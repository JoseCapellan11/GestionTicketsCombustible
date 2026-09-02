namespace GestionTicketsCombustible.Application.Despachos;

public interface IDespachoService
{
    Task<int> RegistrarAsync(CrearDespachoDto dto, int usuarioDespachadorId);

    Task<DespachoDto?> ObtenerPorIdAsync(int id);

    Task<IEnumerable<DespachoDto>> ObtenerTodosAsync();
}