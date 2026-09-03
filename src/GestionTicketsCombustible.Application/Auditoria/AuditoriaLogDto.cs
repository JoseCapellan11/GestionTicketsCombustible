namespace GestionTicketsCombustible.Application.Auditoria;

public class AuditoriaLogDto
{
    public int Id { get; set; }
    public string NombreUsuarioSnapshot { get; set; } = string.Empty;
    public string TipoAccion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string DireccionIp { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
}