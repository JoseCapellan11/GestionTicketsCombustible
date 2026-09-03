namespace GestionTicketsCombustible.Application.CierresDiarios;

public class ConfirmarCierreDiarioDto
{
    public string? Observaciones { get; set; }
    public List<LecturaFisicaDto> LecturasFisicas { get; set; } = new();
}

public class LecturaFisicaDto
{
    public int TanqueId { get; set; }
    public decimal ExistenciaFisica { get; set; }
}