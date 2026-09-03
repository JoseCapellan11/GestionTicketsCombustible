namespace GestionTicketsCombustible.Domain.Entities;

public class AuditoriaLog
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string NombreUsuarioSnapshot { get; set; } = string.Empty;
    public GestionTicketsCombustible.Domain.Enums.TipoAccionAuditoria TipoAccion { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string DireccionIp { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
}