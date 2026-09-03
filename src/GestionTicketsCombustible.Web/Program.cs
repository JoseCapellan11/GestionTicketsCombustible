using GestionTicketsCombustible.Infrastructure.Identity;
using GestionTicketsCombustible.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GestionTicketsCombustible.Application.Departamentos;
using GestionTicketsCombustible.Infrastructure.Persistence.Services;
using GestionTicketsCombustible.Application.Empleados;
using GestionTicketsCombustible.Application.Vehiculos;
using GestionTicketsCombustible.Application.Usuarios;
using GestionTicketsCombustible.Application.Solicitudes;
using GestionTicketsCombustible.Application.Tickets;
using GestionTicketsCombustible.Application.Common;
using GestionTicketsCombustible.Application.Programaciones;
using GestionTicketsCombustible.Infrastructure.BackgroundServices;
using GestionTicketsCombustible.Application.Despachos;
using GestionTicketsCombustible.Application.Inventario;
using GestionTicketsCombustible.Application.Auditoria;
using GestionTicketsCombustible.Application.CierresDiarios;
using QuestPDF.Infrastructure;
using GestionTicketsCombustible.Application.Reportes;
using GestionTicketsCombustible.Application.Dashboard;
using GestionTicketsCombustible.Application.Notificaciones;
using GestionTicketsCombustible.Web.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.Configure<TicketNumeracionOptions>(builder.Configuration.GetSection("TicketNumeracion"));
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.Configure<TicketSeguridadOptions>(builder.Configuration.GetSection("TicketSeguridad"));
builder.Services.AddScoped<ITicketSeguridadService, TicketSeguridadService>();
builder.Services.Configure<AplicacionOptions>(builder.Configuration.GetSection("Aplicacion"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IProgramacionService, ProgramacionService>();
builder.Services.AddHostedService<GeneradorSolicitudesAutomaticasService>();
builder.Services.AddScoped<IDespachoService, DespachoService>();
builder.Services.Configure<AdminSeedOptions>(builder.Configuration.GetSection("AdminInicial"));
builder.Services.AddScoped<ITanqueService, TanqueService>();
builder.Services.AddScoped<IMovimientoInventarioService, MovimientoInventarioService>();
builder.Services.AddScoped<IRecepcionService, RecepcionService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<ICierreDiarioService, CierreDiarioService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();
builder.Services.Configure<NotificacionesOptions>(builder.Configuration.GetSection("Notificaciones"));
builder.Services.AddHostedService<AlertasBackgroundService>();

builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
    await IdentitySeeder.SeedAdminAsync(scope.ServiceProvider);
}

app.Run();