using GestionTicketsCombustible.Domain.Enums;

namespace GestionTicketsCombustible.Domain.Entities;

public class MovimientoInventario
{
    public int Id { get; set; }

    public TipoMovimientoInventario TipoMovimiento { get; set; }
    public SubTipoMovimientoInventario? SubTipo { get; set; }

    public int TanqueId { get; set; }
    public Tanque Tanque { get; set; } = null!;

    public int? TanqueDestinoId { get; set; }
    public Tanque? TanqueDestino { get; set; }

    public decimal Volumen { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string? Referencia { get; set; }

    public int? DespachoId { get; set; }
    public Despacho? Despacho { get; set; }

    public int? RecepcionId { get; set; }
    public RecepcionCombustible? Recepcion { get; set; }
}