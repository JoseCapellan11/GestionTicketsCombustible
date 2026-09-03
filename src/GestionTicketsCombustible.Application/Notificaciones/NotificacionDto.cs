namespace GestionTicketsCombustible.Application.Notificaciones;

public class NotificacionDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? EntidadRelacionada { get; set; }
    public int? EntidadId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Leida { get; set; }
}