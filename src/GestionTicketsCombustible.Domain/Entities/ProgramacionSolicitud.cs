using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Domain.Entities;

public class ProgramacionSolicitud
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public int VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;

    public decimal CantidadAutorizada { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;

    public FrecuenciaProgramacion Frecuencia { get; set; }
    public DateTime? UltimaGeneracion { get; set; }

    public bool Activo { get; set; } = true;
}