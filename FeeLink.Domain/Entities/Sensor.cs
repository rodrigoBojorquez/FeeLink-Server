namespace FeeLink.Domain.Entities;

public class Sensor
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid SensorTypeId { get; set; }
    public SensorType SensorType { get; set; } = null!;
    public Guid ToyId { get; set; }
    public Toy Toy { get; set; } = null!;
}