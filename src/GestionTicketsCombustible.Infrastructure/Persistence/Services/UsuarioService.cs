using GestionTicketsCombustible.Application.Usuarios;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class UsuarioService : IUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsuarioService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await _userManager.Users.ToListAsync();
        var resultado = new List<UsuarioDto>();

        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            resultado.Add(new UsuarioDto
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                UserName = usuario.UserName ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Activo = usuario.LockoutEnd is null || usuario.LockoutEnd < DateTimeOffset.UtcNow,
                Roles = string.Join(", ", roles)
            });
        }

        return resultado;
    }

    public async Task<EditarUsuarioDto?> ObtenerParaEditarAsync(int id)
    {
        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario is null) return null;

        var roles = await _userManager.GetRolesAsync(usuario);
        return new EditarUsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            PhoneNumber = usuario.PhoneNumber,
            Rol = roles.FirstOrDefault() ?? string.Empty
        };
    }

    public async Task<(bool Exito, string[] Errores)> CrearAsync(CrearUsuarioDto dto)
    {
        var usuario = new ApplicationUser
        {
            NombreCompleto = dto.NombreCompleto,
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(usuario, dto.Password);
        if (!resultado.Succeeded)
        {
            return (false, resultado.Errors.Select(e => e.Description).ToArray());
        }

        await _userManager.AddToRoleAsync(usuario, dto.Rol);
        return (true, Array.Empty<string>());
    }

    public async Task ActualizarAsync(EditarUsuarioDto dto)
    {
        var usuario = await _userManager.FindByIdAsync(dto.Id.ToString())
            ?? throw new KeyNotFoundException($"Usuario {dto.Id} no encontrado.");

        usuario.NombreCompleto = dto.NombreCompleto;
        usuario.Email = dto.Email;
        usuario.PhoneNumber = dto.PhoneNumber;
        await _userManager.UpdateAsync(usuario);

        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        if (!rolesActuales.Contains(dto.Rol))
        {
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, dto.Rol);
        }
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        var usuario = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        await _userManager.SetLockoutEndDateAsync(usuario, activo ? null : DateTimeOffset.MaxValue);
    }

    public async Task<List<string>> ObtenerRolesDisponiblesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
    }
}