using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Auditoria;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int? usuarioId,
        string nombreUsuario,
        TipoAccionAuditoria tipoAccion,
        string entidad,
        int? entidadId,
        string descripcion,
        string direccionIp);

    Task<IEnumerable<AuditoriaLogDto>> ObtenerTodosAsync();
}