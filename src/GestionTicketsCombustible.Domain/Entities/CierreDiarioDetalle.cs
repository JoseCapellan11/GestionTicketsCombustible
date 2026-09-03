namespace GestionTicketsCombustible.Domain.Entities;

public class CierreDiarioDetalle
{
    public int Id { get; set; }

    public int CierreDiarioId { get; set; }
    public CierreDiario CierreDiario { get; set; } = null!;

    public int TanqueId { get; set; }
    public Tanque Tanque { get; set; } = null!;

    public decimal ExistenciaTeorica { get; set; }
    public decimal ExistenciaFisica { get; set; }
    public decimal Diferencia { get; set; }

    public decimal DespachadoDia { get; set; }
    public decimal RecibidoDia { get; set; }
}