using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Readings.Common;

public static class ReadingExtensions
{
    public static ReadingResult ToResult(this SensorReading reading)
    {
        return new ReadingResult(
            reading.Id,
            reading.ToyId,
            reading.CreateDate,
            reading.Value,
            reading.Metric.ToString());
    }
}