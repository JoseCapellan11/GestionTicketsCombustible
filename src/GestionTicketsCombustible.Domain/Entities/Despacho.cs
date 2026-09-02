using GestionTicketsCombustible.Domain.Entities;

namespace GestionTicketsCombustible.Domain.Entities;

public class Despacho
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int UsuarioDespachadorId { get; set; }

    public DateTime FechaHoraDespacho { get; set; }

    public decimal GalonesDespachados { get; set; }

    public string Estacion { get; set; } = string.Empty;

    public string? Observaciones { get; set; }
}