using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.Usuarios;
using GestionTicketsCombustible.Domain.Enums;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Services;

public class UsuarioService : IUsuarioService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAuditoriaService _auditoriaService;

    public UsuarioService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IAuditoriaService auditoriaService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _auditoriaService = auditoriaService;
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

    public async Task<(bool Exito, string[] Errores)> CrearAsync(CrearUsuarioDto dto, int actorId, string actorNombreUsuario, string direccionIp)
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

        await _auditoriaService.RegistrarAsync(
            actorId, actorNombreUsuario, TipoAccionAuditoria.Creacion,
            "Usuario", usuario.Id,
            $"Creacion del usuario '{dto.UserName}' con rol '{dto.Rol}'",
            direccionIp);

        return (true, Array.Empty<string>());
    }

    public async Task ActualizarAsync(EditarUsuarioDto dto, int actorId, string actorNombreUsuario, string direccionIp)
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

        await _auditoriaService.RegistrarAsync(
            actorId, actorNombreUsuario, TipoAccionAuditoria.Modificacion,
            "Usuario", usuario.Id,
            $"Actualizacion de datos del usuario '{usuario.UserName}' (rol: '{dto.Rol}')",
            direccionIp);
    }

    public async Task CambiarEstadoAsync(int id, bool activo, int actorId, string actorNombreUsuario, string direccionIp)
    {
        var usuario = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        await _userManager.SetLockoutEndDateAsync(usuario, activo ? null : DateTimeOffset.MaxValue);

        await _auditoriaService.RegistrarAsync(
            actorId, actorNombreUsuario, TipoAccionAuditoria.Modificacion,
            "Usuario", usuario.Id,
            $"Usuario '{usuario.UserName}' {(activo ? "activado" : "desactivado")}",
            direccionIp);
    }

    public async Task<List<string>> ObtenerRolesDisponiblesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
    }

    public async Task<(bool Exito, string[] Errores)> RestablecerPasswordAsync(int id, string nuevaPassword, int actorId, string actorNombreUsuario, string direccionIp)
    {
        var usuario = await _userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"Usuario {id} no encontrado.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
        var resultado = await _userManager.ResetPasswordAsync(usuario, token, nuevaPassword);

        if (!resultado.Succeeded)
        {
            return (false, resultado.Errors.Select(e => e.Description).ToArray());
        }

        await _auditoriaService.RegistrarAsync(
            actorId, actorNombreUsuario, TipoAccionAuditoria.Modificacion,
            "Usuario", usuario.Id,
            $"Restablecimiento de contrasena del usuario '{usuario.UserName}'",
            direccionIp);

        return (true, Array.Empty<string>());
    }
}