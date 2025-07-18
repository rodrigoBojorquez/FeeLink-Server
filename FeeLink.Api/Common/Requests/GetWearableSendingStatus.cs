using Newtonsoft.Json;

namespace FeeLink.Api.Common.Requests;

public class GetWearableSendingStatus
{
    [JsonProperty("userId")] public required string  UserId { get; set; }
    [JsonProperty("macAddress")] public required string MacAddress { get; set; }
    [JsonProperty("enable")] public bool Enable { get; set; } = true;
}