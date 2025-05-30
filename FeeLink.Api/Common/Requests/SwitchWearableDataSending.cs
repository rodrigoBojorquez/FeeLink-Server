using Newtonsoft.Json;

namespace FeeLink.Api.Common.Requests;

public class SwitchWearableDataSending
{
    [JsonProperty("sendData")] public bool SendData { get; set; } = true;
    [JsonProperty("macAddress")] public required string MacAddress { get; set; }

}