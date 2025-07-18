using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FeeLink.Infrastructure.Data.Config;

public class SensorReadingConfig : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.ToyId)
            .IsRequired();

        b.HasOne(x => x.Toy)
            .WithMany(t => t.SensorsReadings)
            .HasForeignKey(x => x.ToyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
