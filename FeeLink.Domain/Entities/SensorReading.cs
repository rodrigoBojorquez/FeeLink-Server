namespace FeeLink.Domain.Entities;

public class SensorReading
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public float Value { get; set; }
}