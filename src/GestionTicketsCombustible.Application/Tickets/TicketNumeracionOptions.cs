namespace GestionTicketsCombustible.Application.Tickets;

// RF-08: prefijo configurable y reinicio anual opcional.
public class TicketNumeracionOptions
{
    public string Prefijo { get; set; } = "COM";
    public bool ReinicioAnual { get; set; } = true;
}