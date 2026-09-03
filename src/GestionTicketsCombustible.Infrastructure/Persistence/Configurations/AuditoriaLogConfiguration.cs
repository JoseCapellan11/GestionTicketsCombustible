using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class AuditoriaLogConfiguration : IEntityTypeConfiguration<AuditoriaLog>
{
    public void Configure(EntityTypeBuilder<AuditoriaLog> builder)
    {
        builder.Property(a => a.TipoAccion).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Entidad).HasMaxLength(50);
        builder.Property(a => a.NombreUsuarioSnapshot).HasMaxLength(150);
        builder.Property(a => a.Descripcion).HasMaxLength(500);
        builder.Property(a => a.DireccionIp).HasMaxLength(45);

        builder.HasIndex(a => a.FechaHora);
        builder.HasIndex(a => a.TipoAccion);
    }
}