using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestionTicketsCombustible.Application.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _opciones;

    public TokenService(IOptions<JwtOptions> opciones)
    {
        _opciones = opciones.Value;
    }

    public string GenerarToken(int usuarioId, string nombreUsuario, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new(ClaimTypes.Name, nombreUsuario)
        };

        claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.ClaveSecreta));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opciones.Issuer,
            audience: _opciones.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_opciones.ExpiracionMinutos),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}