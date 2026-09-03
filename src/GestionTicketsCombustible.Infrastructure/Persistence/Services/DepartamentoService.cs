using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class DepartamentoService : IDepartamentoService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public DepartamentoService(ApplicationDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<List<DepartamentoDto>> ObtenerTodosAsync()
    {
        return await _context.Departamentos
            .Select(d => new DepartamentoDto { Id = d.Id, Nombre = d.Nombre, Activo = d.Activo })
            .ToListAsync();
    }

    public async Task<DepartamentoDto?> ObtenerPorIdAsync(int id)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento is null) return null;
        return new DepartamentoDto { Id = departamento.Id, Nombre = departamento.Nombre, Activo = departamento.Activo };
    }

    public async Task<int> CrearAsync(string nombre, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var departamento = new Departamento { Nombre = nombre, Activo = true };
        _context.Departamentos.Add(departamento);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Creacion,
            "Departamento", departamento.Id,
            $"Creacion del departamento '{departamento.Nombre}'",
            direccionIp);

        return departamento.Id;
    }

    public async Task ActualizarAsync(int id, string nombre, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento is null) throw new KeyNotFoundException($"Departamento {id} no encontrado.");
        var nombreAnterior = departamento.Nombre;
        departamento.Nombre = nombre;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Modificacion,
            "Departamento", departamento.Id,
            $"Departamento renombrado de '{nombreAnterior}' a '{departamento.Nombre}'",
            direccionIp);
    }

    public async Task CambiarEstadoAsync(int id, bool activo, int usuarioId, string nombreUsuario, string direccionIp)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento is null) throw new KeyNotFoundException($"Departamento {id} no encontrado.");
        departamento.Activo = activo;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioId, nombreUsuario, TipoAccionAuditoria.Modificacion,
            "Departamento", departamento.Id,
            $"Departamento '{departamento.Nombre}' {(activo ? "activado" : "desactivado")}",
            direccionIp);
    }
}