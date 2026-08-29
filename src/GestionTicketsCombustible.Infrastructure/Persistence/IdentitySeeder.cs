using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GestionTicketsCombustible.Infrastructure.Persistence;

public static class IdentitySeeder
{
    private static readonly string[] RolesRequeridos =
    {
        Roles.Administrador,
        Roles.Supervisor,
        Roles.Despachador,
        Roles.Auditor,
        Roles.Consulta
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
}