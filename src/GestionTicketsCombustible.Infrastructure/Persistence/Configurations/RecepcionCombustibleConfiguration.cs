using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class RecepcionCombustibleConfiguration : IEntityTypeConfiguration<RecepcionCombustible>
{
    public void Configure(EntityTypeBuilder<RecepcionCombustible> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rnc).HasMaxLength(20).IsRequired();
        builder.Property(r => r.NombreSuplidor).HasMaxLength(150).IsRequired();
        builder.Property(r => r.Factura).HasMaxLength(50).IsRequired();
        builder.Property(r => r.VolumenRecibido).HasPrecision(12, 2);

        builder.HasOne(r => r.Tanque)
            .WithMany()
            .HasForeignKey(r => r.TanqueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}