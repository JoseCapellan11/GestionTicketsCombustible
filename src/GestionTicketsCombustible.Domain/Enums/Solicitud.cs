using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Domain.Entities;

public class Solicitud
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public int VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;

    // Solo el FK crudo: Domain no puede conocer ApplicationUser (vive en Infrastructure).
    public int? UsuarioSolicitanteId { get; set; }

    public decimal CantidadAutorizada { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;

    public TipoSolicitud TipoSolicitud { get; set; } = TipoSolicitud.Manual;
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public DateTime FechaSolicitud { get; set; } = DateTime.Now;
    public DateTime FechaVencimiento { get; set; }

    public Ticket? Ticket { get; set; }
}