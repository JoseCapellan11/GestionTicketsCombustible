using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Solicitudes;

public class SolicitudDto
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; } = string.Empty;

    public int VehiculoId { get; set; }
    public string VehiculoPlaca { get; set; } = string.Empty;

    public int DepartamentoId { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;

    public decimal CantidadAutorizada { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;

    public TipoSolicitud TipoSolicitud { get; set; }
    public EstadoSolicitud Estado { get; set; }

    public DateTime FechaSolicitud { get; set; }
    public DateTime FechaVencimiento { get; set; }
}