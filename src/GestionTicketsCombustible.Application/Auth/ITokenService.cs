namespace GestionTicketsCombustible.Application.Auth;

public interface ITokenService
{
    string GenerarToken(int usuarioId, string nombreUsuario, IEnumerable<string> roles);
}