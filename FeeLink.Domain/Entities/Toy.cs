namespace FeeLink.Domain.Entities;

public class Toy
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    
    public List<Sensor> Sensors { get; set; } = [];
}