using System.Text.Json.Serialization;

namespace FeeLink.Infrastructure.Common.Wearable;


public class SaveWearableData
{
    [JsonPropertyName("p")] public Pressure? P { get; set; }

    [JsonPropertyName("b")] public float? B { get; set; }

    [JsonPropertyName("m")] public AxisData? M { get; set; }

    [JsonPropertyName("g")] public AxisData? G { get; set; }
    [JsonPropertyName("w")] public bool ? W { get; set; }   
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