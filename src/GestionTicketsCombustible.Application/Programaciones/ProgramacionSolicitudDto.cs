using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Application.Programaciones;

public class ProgramacionSolicitudDto
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
    public FrecuenciaProgramacion Frecuencia { get; set; }
    public DateTime? UltimaGeneracion { get; set; }
    public bool Activo { get; set; }
}