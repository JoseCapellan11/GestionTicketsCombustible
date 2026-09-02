namespace GestionTicketsCombustible.Domain.Entities;

public class RecepcionCombustible
{
    public int Id { get; set; }

    public string Rnc { get; set; } = string.Empty;
    public string NombreSuplidor { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public decimal VolumenRecibido { get; set; }
    public DateTime Fecha { get; set; }

    public int TanqueId { get; set; }
    public Tanque Tanque { get; set; } = null!;
}