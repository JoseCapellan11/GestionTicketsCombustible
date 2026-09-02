namespace GestionTicketsCombustible.Application.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string ClaveSecreta { get; set; } = string.Empty;
    public int ExpiracionMinutos { get; set; } = 120;
}