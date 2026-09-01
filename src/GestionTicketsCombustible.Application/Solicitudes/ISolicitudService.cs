namespace GestionTicketsCombustible.Application.Solicitudes;

public interface ISolicitudService
{
    Task<IEnumerable<SolicitudDto>> ObtenerTodasAsync();
    Task<SolicitudDto?> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(CrearSolicitudDto dto, int? usuarioSolicitanteId);
    Task AprobarAsync(int id);
    Task RechazarAsync(int id);
}