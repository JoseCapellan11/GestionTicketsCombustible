namespace GestionTicketsCombustible.Application.Inventario;

public class TanqueDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string TipoCombustible { get; set; } = string.Empty;
    public decimal Capacidad { get; set; }
    public decimal ExistenciaActual { get; set; }
    public decimal Disponibilidad { get; set; }
    public decimal NivelCritico { get; set; }
    public bool EnNivelCritico { get; set; }
    public bool Activo { get; set; }
}