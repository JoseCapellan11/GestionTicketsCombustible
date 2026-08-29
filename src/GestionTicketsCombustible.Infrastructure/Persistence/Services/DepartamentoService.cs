using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class DepartamentoService : IDepartamentoService
{
    private readonly ApplicationDbContext _context;

    public DepartamentoService(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<int> CrearAsync(string nombre)
    {
        var departamento = new Departamento { Nombre = nombre, Activo = true };
        _context.Departamentos.Add(departamento);
        await _context.SaveChangesAsync();
        return departamento.Id;
    }

    public async Task ActualizarAsync(int id, string nombre)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento is null) throw new KeyNotFoundException($"Departamento {id} no encontrado.");
        departamento.Nombre = nombre;
        await _context.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        var departamento = await _context.Departamentos.FindAsync(id);
        if (departamento is null) throw new KeyNotFoundException($"Departamento {id} no encontrado.");
        departamento.Activo = activo;
        await _context.SaveChangesAsync();
    }
}