namespace GestionTicketsCombustible.Domain.Entities;

public class CierreDiario
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }
    public DateTime FechaHoraCierre { get; set; } = DateTime.Now;

    public int UsuarioCierreId { get; set; }
    public string UsuarioCierreNombreSnapshot { get; set; } = string.Empty;

    public decimal TotalDespachadoDia { get; set; }
    public decimal TotalRecibidoDia { get; set; }

    public string? Observaciones { get; set; }

    public ICollection<CierreDiarioDetalle> Detalles { get; set; } = new List<CierreDiarioDetalle>();
}