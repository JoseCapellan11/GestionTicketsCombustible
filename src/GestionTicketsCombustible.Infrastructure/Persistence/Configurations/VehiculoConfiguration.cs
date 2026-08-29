using GestionTicketsCombustible.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionTicketsCombustible.Infrastructure.Persistence.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.Property(v => v.Placa).IsRequired().HasMaxLength(20);
        builder.HasIndex(v => v.Placa).IsUnique();

        builder.Property(v => v.Ficha).HasMaxLength(20);
        builder.Property(v => v.Marca).HasMaxLength(50);
        builder.Property(v => v.Modelo).HasMaxLength(50);
        builder.Property(v => v.CapacidadTanque).HasColumnType("decimal(10,2)");

        builder.HasOne(v => v.Departamento)
            .WithMany(d => d.Vehiculos)
            .HasForeignKey(v => v.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}