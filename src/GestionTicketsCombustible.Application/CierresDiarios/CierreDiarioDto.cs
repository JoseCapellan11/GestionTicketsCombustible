namespace GestionTicketsCombustible.Application.CierresDiarios;

public class CierreDiarioDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaHoraCierre { get; set; }
    public string UsuarioCierreNombreSnapshot { get; set; } = string.Empty;
    public decimal TotalDespachadoDia { get; set; }
    public decimal TotalRecibidoDia { get; set; }
    public string? Observaciones { get; set; }
    public List<CierreDiarioDetalleDto> Detalles { get; set; } = new();
}

public class CierreDiarioDetalleDto
{
    public int TanqueId { get; set; }
    public string TanqueNombre { get; set; } = string.Empty;
    public decimal ExistenciaTeorica { get; set; }
    public decimal ExistenciaFisica { get; set; }
    public decimal Diferencia { get; set; }
    public decimal DespachadoDia { get; set; }
    public decimal RecibidoDia { get; set; }
}