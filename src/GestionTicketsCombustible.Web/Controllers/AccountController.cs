using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Domain.Enums;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuditoriaService _auditoriaService;

    public AccountController(SignInManager<ApplicationUser> signInManager, IAuditoriaService auditoriaService)
    {
        _signInManager = signInManager;
        _auditoriaService = auditoriaService;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string usuario, string password, string? returnUrl = null)
    {
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
        var usuarioEncontrado = await _signInManager.UserManager.FindByNameAsync(usuario);

        var resultado = await _signInManager.PasswordSignInAsync(usuario, password, isPersistent: false, lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            await _auditoriaService.RegistrarAsync(
                usuarioEncontrado?.Id, usuario, TipoAccionAuditoria.Acceso,
                "Sesion", usuarioEncontrado?.Id, "Login exitoso (Web)", direccionIp);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        var descripcion = resultado.IsLockedOut
            ? "Login fallido: cuenta bloqueada temporalmente por intentos fallidos"
            : "Login fallido: usuario o contrasena incorrectos";

        await _auditoriaService.RegistrarAsync(
            usuarioEncontrado?.Id, usuario, TipoAccionAuditoria.Acceso,
            "Sesion", usuarioEncontrado?.Id, descripcion, direccionIp);

        if (resultado.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por intentos fallidos.");
        else
            ModelState.AddModelError(string.Empty, "Usuario o contrasena incorrectos.");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}