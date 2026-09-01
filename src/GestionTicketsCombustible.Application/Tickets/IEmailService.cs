using GestionTicketsCombustible.Application.Tickets;

namespace GestionTicketsCombustible.Application.Tickets;

public interface IEmailService
{
    Task EnviarTicketAsync(TicketDto ticket, string correoDestino, byte[] qrPng, string urlValidacion);
}