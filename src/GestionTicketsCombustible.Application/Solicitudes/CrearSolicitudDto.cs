using System.ComponentModel.DataAnnotations;
using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Solicitudes;

public class CrearSolicitudDto
{
    [Required(ErrorMessage = "Selecciona un empleado.")]
    public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "Selecciona un vehiculo.")]
    public int VehiculoId { get; set; }

    [Required(ErrorMessage = "Selecciona un departamento.")]
    public int DepartamentoId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public decimal CantidadAutorizada { get; set; }

    [Required(ErrorMessage = "Indica el tipo de combustible.")]
    [StringLength(50)]
    public string TipoCombustible { get; set; } = string.Empty;

    public TipoSolicitud TipoSolicitud { get; set; } = TipoSolicitud.Manual;

    [Required(ErrorMessage = "Indica la fecha de vencimiento.")]
    public DateTime FechaVencimiento { get; set; }
}