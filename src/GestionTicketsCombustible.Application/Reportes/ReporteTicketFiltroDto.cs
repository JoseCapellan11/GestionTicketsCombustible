using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Reportes;

public class ReporteTicketFiltroDto
{
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int? EmpleadoId { get; set; }
    public int? VehiculoId { get; set; }
    public int? DepartamentoId { get; set; }
    public string? TipoCombustible { get; set; }
    public EstadoTicket? Estado { get; set; }
}