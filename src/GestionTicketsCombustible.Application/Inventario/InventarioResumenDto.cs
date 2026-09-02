namespace GestionTicketsCombustible.Application.Inventario;

public class InventarioResumenDto
{
    public int TanqueId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoCombustible { get; set; } = string.Empty;
    public decimal Capacidad { get; set; }
    public decimal ExistenciaActual { get; set; }
    public decimal Disponibilidad { get; set; }
    public decimal NivelCritico { get; set; }
    public bool EnNivelCritico { get; set; }
    public decimal ConsumoDiario { get; set; }
    public decimal ConsumoMensual { get; set; }
}