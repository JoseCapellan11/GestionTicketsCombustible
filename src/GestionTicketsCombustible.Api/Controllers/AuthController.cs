using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Auth;
using GestionTicketsCombustible.Domain.Enums;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GestionTicketsCombustible.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;
    private readonly IAuditoriaService _auditoriaService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IAuditoriaService auditoriaService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
        _auditoriaService = auditoriaService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        var usuario = await _userManager.FindByNameAsync(dto.NombreUsuario);

        if (usuario is null)
        {
            await _auditoriaService.RegistrarAsync(
                null, dto.NombreUsuario, TipoAccionAuditoria.Acceso,
                "Sesion", null, "Login fallido (Api): usuario no existe", direccionIp);
            return Unauthorized("Usuario o contrasena incorrectos.");
        }

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Password, lockoutOnFailure: true);
        if (!resultado.Succeeded)
        {
            var descripcion = resultado.IsLockedOut
                ? "Login fallido (Api): cuenta bloqueada temporalmente por intentos fallidos"
                : "Login fallido (Api): usuario o contrasena incorrectos";

            await _auditoriaService.RegistrarAsync(
                usuario.Id, dto.NombreUsuario, TipoAccionAuditoria.Acceso,
                "Sesion", usuario.Id, descripcion, direccionIp);
            return Unauthorized("Usuario o contrasena incorrectos.");
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        var token = _tokenService.GenerarToken(usuario.Id, usuario.UserName!, roles);

        await _auditoriaService.RegistrarAsync(
            usuario.Id, dto.NombreUsuario, TipoAccionAuditoria.Acceso,
            "Sesion", usuario.Id, "Login exitoso (Api)", direccionIp);

        return Ok(new TokenResponseDto
        {
            Token = token,
            Expira = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiracionMinutos)
        });
    }
}