using System.Text.Json.Serialization;
using FeeLink.Api.WebSockets;
using FeeLink.Domain.Entities;

namespace FeeLink.Api.Common.Requests;

public record SensorData(float Value, Metric Metric);

public class SaveWearableData
{
    [JsonPropertyName("p")] public Pressure? P { get; set; }

    [JsonPropertyName("b")] public float? B { get; set; }

    [JsonPropertyName("m")] public AxisData? M { get; set; }

    [JsonPropertyName("g")] public AxisData? G { get; set; }

}

public class Pressure
{
    [JsonPropertyName("pc")] public float? Pc { get; set; }

    [JsonPropertyName("gr")] public float? Gr { get; set; }
}

public class AxisData
{
    [JsonPropertyName("x")] public float? X { get; set; }

    [JsonPropertyName("y")] public float? Y { get; set; }

    [JsonPropertyName("z")] public float? Z { get; set; }
}