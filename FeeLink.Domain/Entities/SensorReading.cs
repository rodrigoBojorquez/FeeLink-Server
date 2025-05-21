namespace FeeLink.Domain.Entities;

public class SensorReading
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ToyId { get; set; }
    public Toy Toy { get; set; } = null!;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public float Value { get; set; }
    public Metric Metric { get; set; }
}

public enum Metric
{
    PressurePercent,
    PressureGram,
    Battery,
    Accel,
    Gyro,
    Wifi
}