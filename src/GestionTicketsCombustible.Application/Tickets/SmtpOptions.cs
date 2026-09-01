namespace GestionTicketsCombustible.Application.Tickets;

public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Puerto { get; set; } = 587;
    public bool UsarSsl { get; set; } = true;
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RemitenteNombre { get; set; } = "Gestion de Combustible";
    public string RemitenteCorreo { get; set; } = string.Empty;
}