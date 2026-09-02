using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.TipoMovimiento).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.SubTipo).HasConversion<string>().HasMaxLength(30);

        builder.Property(m => m.Volumen).HasPrecision(12, 2);

        builder.Property(m => m.Referencia).HasMaxLength(300);

        builder.HasOne(m => m.Tanque)
            .WithMany()
            .HasForeignKey(m => m.TanqueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.TanqueDestino)
            .WithMany()
            .HasForeignKey(m => m.TanqueDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Despacho)
            .WithMany()
            .HasForeignKey(m => m.DespachoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Recepcion)
            .WithMany()
            .HasForeignKey(m => m.RecepcionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}