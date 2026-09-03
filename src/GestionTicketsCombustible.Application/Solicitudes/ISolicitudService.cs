namespace GestionTicketsCombustible.Application.Solicitudes;

public interface ISolicitudService
{
    Task<IEnumerable<SolicitudDto>> ObtenerTodasAsync();
    Task<SolicitudDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearSolicitudDto dto, int? usuarioSolicitanteId);
    Task AprobarAsync(int id, int usuarioId, string nombreUsuario, string direccionIp);
    Task RechazarAsync(int id, int usuarioId, string nombreUsuario, string direccionIp);
}