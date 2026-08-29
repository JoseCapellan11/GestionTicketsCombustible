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
        var (exito, errores) = await _usuarioService.CrearAsync(dto);
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
        await _usuarioService.ActualizarAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo)
    {
        await _usuarioService.CambiarEstadoAsync(id, activo);
        return RedirectToAction(nameof(Index));
    }
}