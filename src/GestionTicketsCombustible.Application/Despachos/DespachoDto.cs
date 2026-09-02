namespace GestionTicketsCombustible.Application.Despachos;

public class DespachoDto
{
    public int Id { get; set; }

    public int TicketId { get; set; }
    public string NumeroTicket { get; set; } = string.Empty;
    public string EmpleadoNombreSnapshot { get; set; } = string.Empty;
    public string VehiculoPlacaSnapshot { get; set; } = string.Empty;

    public int UsuarioDespachadorId { get; set; }
    public string NombreDespachador { get; set; } = string.Empty;

    public int TanqueId { get; set; }
    public string NombreTanque { get; set; } = string.Empty;

    public DateTime FechaHoraDespacho { get; set; }
    public decimal GalonesDespachados { get; set; }
    public string Estacion { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}