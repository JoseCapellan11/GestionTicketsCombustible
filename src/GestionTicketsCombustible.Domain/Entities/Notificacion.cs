using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Domain.Entities;

public class Notificacion
{
    public int Id { get; set; }

    public TipoNotificacion Tipo { get; set; }
    public string Mensaje { get; set; } = string.Empty;

    // Referencia opcional a la entidad que origino la alerta (ej. "Ticket", 7)
    public string? EntidadRelacionada { get; set; }
    public int? EntidadId { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Leida { get; set; }
    public DateTime? FechaLectura { get; set; }
}