namespace GestionTicketsCombustible.Application.Inventario;

public class RecepcionCombustibleDto
{
    public int Id { get; set; }
    public string Rnc { get; set; } = string.Empty;
    public string NombreSuplidor { get; set; } = string.Empty;
    public string Factura { get; set; } = string.Empty;
    public decimal VolumenRecibido { get; set; }
    public DateTime Fecha { get; set; }
    public int TanqueId { get; set; }
    public string TanqueNombre { get; set; } = string.Empty;
}