namespace GestionTicketsCombustible.Application.CierresDiarios;

public interface ICierreDiarioService
{
    Task<IEnumerable<CierreDiarioDto>> ObtenerTodosAsync();
    Task<CierreDiarioDto?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteCierreHoyAsync();
    Task<PrepararCierreDiarioDto> PrepararCierreAsync();
    Task<int> ConfirmarCierreAsync(ConfirmarCierreDiarioDto dto, int usuarioId, string nombreUsuario, string direccionIp);
    Task<byte[]?> GenerarActaPdfAsync(int cierreId);
}