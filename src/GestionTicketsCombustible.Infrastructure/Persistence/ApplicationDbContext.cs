using GestionTicketsCombustible.Domain.Entities;
using GestionTicketsCombustible.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketsCombustible.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ProgramacionSolicitud> ProgramacionesSolicitud { get; set; }

    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<Solicitud> Solicitudes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<SecuenciaTicket> SecuenciasTicket { get; set; }
    public DbSet<Despacho> Despachos { get; set; }
    public DbSet<Tanque> Tanques { get; set; }
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
    public DbSet<RecepcionCombustible> RecepcionesCombustible { get; set; }
    public DbSet<AuditoriaLog> AuditoriaLogs { get; set; }
    public DbSet<CierreDiario> CierresDiarios { get; set; }
    public DbSet<CierreDiarioDetalle> CierreDiarioDetalles { get; set; }
}