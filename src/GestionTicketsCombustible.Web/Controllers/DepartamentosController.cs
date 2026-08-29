using GestionTicketsCombustible.Application.Departamentos;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketsCombustible.Web.Controllers;

public class DepartamentosController : Controller
{
    private readonly IDepartamentoService _departamentoService;

    public DepartamentosController(IDepartamentoService departamentoService)
    {
        _departamentoService = departamentoService;
    }

    public async Task<IActionResult> Index()
    {
        var departamentos = await _departamentoService.ObtenerTodosAsync();
        return View(departamentos);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(nameof(nombre), "El nombre es obligatorio.");
            return View();
        }
        await _departamentoService.CrearAsync(nombre);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var departamento = await _departamentoService.ObtenerPorIdAsync(id);
        if (departamento is null) return NotFound();
        return View(departamento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            ModelState.AddModelError(nameof(nombre), "El nombre es obligatorio.");
            return View(new DepartamentoDto { Id = id, Nombre = nombre });
        }
        await _departamentoService.ActualizarAsync(id, nombre);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, bool activo)
    {
        await _departamentoService.CambiarEstadoAsync(id, activo);
        return RedirectToAction(nameof(Index));
    }
}