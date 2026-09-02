namespace GestionTicketsCombustible.Application.Auth;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
}