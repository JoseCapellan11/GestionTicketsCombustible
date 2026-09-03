using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Tickets;

public class TicketDto
{
    public int Id { get; set; }
    public Guid TicketUid { get; set; }
    public string NumeroTicket { get; set; } = string.Empty;

    public int SolicitudId { get; set; }

    public int EmpleadoId { get; set; }
    public int VehiculoId { get; set; }
    public int DepartamentoId { get; set; }

    public string EmpleadoNombreSnapshot { get; set; } = string.Empty;
    public string VehiculoPlacaSnapshot { get; set; } = string.Empty;
    public string DepartamentoNombreSnapshot { get; set; } = string.Empty;

    public decimal CantidadAutorizada { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaVencimiento { get; set; }

    public EstadoTicket Estado { get; set; }
    public string EstadoVisual { get; set; } = string.Empty;
    public bool? HashValido { get; set; }

    public string? MotivoAnulacion { get; set; }
    public DateTime? FechaAnulacion { get; set; }
}