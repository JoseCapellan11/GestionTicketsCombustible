using GestionTicketsCombustible.Application.Auth;
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

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService, IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _userManager.FindByNameAsync(dto.NombreUsuario);
        if (usuario is null)
            return Unauthorized("Usuario o contrasena incorrectos.");

        var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Password, lockoutOnFailure: true);
        if (!resultado.Succeeded)
            return Unauthorized("Usuario o contrasena incorrectos.");

        var roles = await _userManager.GetRolesAsync(usuario);
        var token = _tokenService.GenerarToken(usuario.Id, usuario.UserName!, roles);

        return Ok(new TokenResponseDto
        {
            Token = token,
            Expira = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiracionMinutos)
        });
    }
}