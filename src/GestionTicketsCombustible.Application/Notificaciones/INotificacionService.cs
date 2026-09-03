using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Notificaciones;

public interface INotificacionService
{
    Task<List<NotificacionDto>> ObtenerRecientesAsync(int cantidad = 50);
    Task<int> ContarNoLeidasAsync();
    Task MarcarComoLeidaAsync(int id);
    Task MarcarTodasComoLeidasAsync();
    Task CrearAsync(TipoNotificacion tipo, string mensaje, string? entidadRelacionada = null, int? entidadId = null);
    Task<bool> ExisteNotificacionNoLeidaAsync(TipoNotificacion tipo, string entidadRelacionada, int entidadId);
}