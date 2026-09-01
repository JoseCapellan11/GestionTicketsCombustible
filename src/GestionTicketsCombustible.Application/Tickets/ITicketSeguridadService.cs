namespace GestionTicketsCombustible.Application.Tickets;

public interface ITicketSeguridadService
{
    string GenerarHash(Guid ticketUid, string numeroTicket, string empleadoNombre, string vehiculoPlaca,
        decimal cantidadCombustible, DateTime fechaEmision, DateTime fechaExpiracion);

    bool VerificarHash(string hashEsperado, Guid ticketUid, string numeroTicket, string empleadoNombre,
        string vehiculoPlaca, decimal cantidadCombustible, DateTime fechaEmision, DateTime fechaExpiracion);

    string GenerarTokenValidacion();
}