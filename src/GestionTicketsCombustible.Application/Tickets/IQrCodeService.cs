namespace GestionTicketsCombustible.Application.Tickets;

public interface IQrCodeService
{
    byte[] GenerarPng(string contenido);
}