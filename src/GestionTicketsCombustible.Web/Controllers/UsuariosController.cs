using System.Security.Claims;
using GestionTicketsCombustible.Application.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionTicketsCombustible.Web.Controllers;

public class UsuariosController : Controller
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    private async Task CargarRolesAsync(string? seleccionado = null)
    {
        var roles = await _usuarioService.ObtenerRolesDisponiblesAsync();
        ViewBag.Roles = new SelectList(roles, seleccionado);
    }

    public async Task<IActionResult> Index()
    {
        return View(await _usuarioService.ObtenerTodosAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarRolesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearUsuarioDto dto)
    {
        var actorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        var (exito, errores) = await _usuarioService.CrearAsync(dto, actorId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        if (!exito)
        {
            foreach (var error in errores)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            await CargarRolesAsync(dto.Rol);
            return View(dto);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _usuarioService.ObtenerParaEditarAsync(id);
        if (usuario is null) return NotFound();
        await CargarRolesAsync(usuario.Rol);
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditarUsuarioDto dto)
    {
        var actorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _usuarioService.ActualizarAsync(dto, actorId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo)
    {
        var actorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        await _usuarioService.CambiarEstadoAsync(id, activo, actorId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult ResetPassword(int id)
    {
        ViewBag.UsuarioId = id;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int id, string nuevaPassword)
    {
        var actorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var direccionIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "desconocida";

        var (exito, errores) = await _usuarioService.RestablecerPasswordAsync(id, nuevaPassword, actorId, User.Identity?.Name ?? "(desconocido)", direccionIp);
        if (!exito)
        {
            foreach (var error in errores)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ViewBag.UsuarioId = id;
            return View();
        }
        return RedirectToAction(nameof(Index));
    }
}