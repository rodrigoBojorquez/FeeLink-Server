using System.Threading.Channels;

namespace FeeLink.Infrastructure.Common.Wearable;

/// <summary>
/// Queue for decoupling sensor data ingestion from persistence.
/// </summary>
public class SensorDataQueue
{
    /// <summary>
    /// Underlying channel for sensor data items.
    /// </summary>
    public Channel<SensorDataQueueItem> Queue { get; } = Channel.CreateUnbounded<SensorDataQueueItem>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
}

/// <summary>
/// Item representing a batchable unit of sensor data.
/// </summary>
public record SensorDataQueueItem(
    Guid ToyId,
    SaveWearableData Data,
    string Identifier,
    IEnumerable<string> UserIds);