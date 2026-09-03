namespace GestionTicketsCombustible.Application.Tickets;

public interface ITicketService
{
    Task<IEnumerable<TicketDto>> ObtenerTodosAsync();
    Task<TicketDto?> ObtenerPorIdAsync(int id);
    Task<TicketDto?> ObtenerPorTokenAsync(string token);
    Task<TicketDto> GenerarDesdeSolicitudAsync(int solicitudId, int usuarioId, string nombreUsuario, string direccionIp);
    Task EnviarPorCorreoAsync(int ticketId);
    Task<byte[]?> GenerarImagenQrAsync(int ticketId);
    Task<byte[]?> GenerarImagenQrPorTokenAsync(string token);
    Task AnularAsync(int ticketId, string motivo, int usuarioId, string nombreUsuario, string direccionIp);
    Task<byte[]?> GenerarPdfAsync(int ticketId);
}