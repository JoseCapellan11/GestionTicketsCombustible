using System.ComponentModel.DataAnnotations;
using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Programaciones;

public class CrearProgramacionSolicitudDto
{
    [Required] public int EmpleadoId { get; set; }
    [Required] public int VehiculoId { get; set; }
    [Required] public int DepartamentoId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public decimal CantidadAutorizada { get; set; }

    [Required, StringLength(50)]
    public string TipoCombustible { get; set; } = string.Empty;

    public FrecuenciaProgramacion Frecuencia { get; set; } = FrecuenciaProgramacion.Mensual;
}