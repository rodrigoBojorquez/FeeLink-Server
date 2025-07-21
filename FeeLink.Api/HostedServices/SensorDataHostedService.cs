using ErrorOr;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Infrastructure.Common.Wearable;
using MediatR;

namespace FeeLink.Api.HostedServices;

/// <summary>
/// Background service that drains the sensor data queue and persists to database.
/// </summary>
public class SensorDataHostedService : BackgroundService
{
    private readonly SensorDataQueue _queue;
    private readonly IMediator _mediator;

    public SensorDataHostedService(SensorDataQueue queue, IMediator mediator)
    {
        _queue = queue;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _queue.Queue.Reader.WaitToReadAsync(stoppingToken))
        {
            var item = await _queue.Queue.Reader.ReadAsync(stoppingToken);

            // Convert to domain SensorData and persist
            var sensorDataList = item.Data.ToSensorData();
            var tasks = new List<Task<ErrorOr<Created>>>();

            foreach (var sensor in sensorDataList)
            {
                tasks.Add(_mediator.Send(
                    new SaveSensorDataCommand(item.ToyId, sensor.Value, sensor.Metric), stoppingToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}