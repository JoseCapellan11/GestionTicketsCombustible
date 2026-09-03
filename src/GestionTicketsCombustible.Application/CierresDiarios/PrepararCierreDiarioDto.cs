namespace GestionTicketsCombustible.Application.CierresDiarios;

public class PrepararCierreDiarioDto
{
    public DateTime Fecha { get; set; }
    public List<PrepararCierreDetalleDto> Tanques { get; set; } = new();
}

public class PrepararCierreDetalleDto
{
    public int TanqueId { get; set; }
    public string TanqueNombre { get; set; } = string.Empty;
    public decimal ExistenciaTeorica { get; set; }
    public decimal DespachadoDia { get; set; }
    public decimal RecibidoDia { get; set; }
}