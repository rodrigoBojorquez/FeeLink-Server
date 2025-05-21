using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FeeLink.Infrastructure.Data.Config;

public class SensorReadingConfig : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        var sensorReadingMetricConverter = new EnumToStringConverter<Metric>();
        
        builder.Property(s => s.Metric)
            .HasConversion(sensorReadingMetricConverter);
    }
}