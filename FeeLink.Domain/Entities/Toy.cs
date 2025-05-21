namespace FeeLink.Domain.Entities;

public class Toy
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    
    public List<SensorReading> SensorsReadings { get; set; } = [];
}