namespace GestionTicketsCombustible.Application.Inventario;

public class MovimientoInventarioDto
{
    public int Id { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public string? SubTipo { get; set; }
    public string TanqueNombre { get; set; } = string.Empty;
    public string? TanqueDestinoNombre { get; set; }
    public decimal Volumen { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string? Referencia { get; set; }
}