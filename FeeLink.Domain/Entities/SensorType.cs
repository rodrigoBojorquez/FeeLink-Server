namespace FeeLink.Domain.Entities;

public class SensorType
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Type { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}