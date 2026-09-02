using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GestionTicketsCombustible.Infrastructure.Persistence;

public static class IdentitySeeder
{
    private static readonly string[] RolesRequeridos =
    {
        Roles.Administrador,
        Roles.Supervisor,
        Roles.Despachador,
        Roles.Auditor,
        Roles.Consulta,
        Roles.Solicitante
    };

    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var nombreRol in RolesRequeridos)
        {
            if (!await roleManager.RoleExistsAsync(nombreRol))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = nombreRol });
            }
        }
    }

    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var opciones = serviceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

        if (string.IsNullOrWhiteSpace(opciones.Password))
            return;

        var usuariosAdmin = await userManager.GetUsersInRoleAsync(Roles.Administrador);
        if (usuariosAdmin.Count > 0)
            return;

        var admin = new ApplicationUser
        {
            UserName = opciones.UserName,
            Email = opciones.Email,
            NombreCompleto = opciones.NombreCompleto,
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(admin, opciones.Password);
        if (resultado.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.Administrador);
        }
    }
}