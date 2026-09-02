namespace GestionTicketsCombustible.Application.Inventario;

public interface IRecepcionService
{
    Task<int> RegistrarAsync(CrearRecepcionCombustibleDto dto);
    Task<IEnumerable<RecepcionCombustibleDto>> ObtenerTodasAsync();
}