using Newtonsoft.Json;

namespace FeeLink.Api.Common.Requests;

public class DisconnectWifi
{
    [JsonProperty("macAddress")] public required string MacAddress { get; set; }
}