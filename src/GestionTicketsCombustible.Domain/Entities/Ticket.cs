using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Domain.Entities;

public class Ticket
{
    public int Id { get; set; }

    public Guid TicketUid { get; set; } = Guid.NewGuid();   // RF-06: Identificador único (UUID)
    public string NumeroTicket { get; set; } = string.Empty; // RF-06/RF-08: ej. COM-2026-000001

    public int SolicitudId { get; set; }
    public Solicitud Solicitud { get; set; } = null!;

    public Despacho? Despacho { get; set; }

    public int EmpleadoId { get; set; }
    public Empleado Empleado { get; set; } = null!;

    public int VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;

    // Snapshot inmutable al momento de emisión (ver explicación arriba).
    public string EmpleadoNombreSnapshot { get; set; } = string.Empty;
    public string VehiculoPlacaSnapshot { get; set; } = string.Empty;
    public string DepartamentoNombreSnapshot { get; set; } = string.Empty;

    public decimal CantidadAutorizada { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime FechaVencimiento { get; set; }

    public EstadoTicket Estado { get; set; } = EstadoTicket.Creado;

    // RF-07 / RS-04: seguridad del QR (se calculan en la Fase 2.5)
    public string HashSeguridad { get; set; } = string.Empty;
    public string TokenValidacion { get; set; } = string.Empty;
}