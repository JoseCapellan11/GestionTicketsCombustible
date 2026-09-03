namespace GestionTicketsCombustible.Application.Inventario;

public interface IRecepcionService
{
    Task<int> RegistrarAsync(CrearRecepcionCombustibleDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task<IEnumerable<RecepcionCombustibleDto>> ObtenerTodasAsync();
}