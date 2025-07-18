using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Sensors.Commands.SaveData;

public class SaveSensorDataCommandHandler(ISensorReadingRepository sensorReadingRepository) : IRequestHandler<SaveSensorDataCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(SaveSensorDataCommand request, CancellationToken cancellationToken)
    {
        var sensorReading = new SensorReading
        {
            ToyId = request.ToyId,
            Value = request.Value,
            Metric = request.Metric,
        };

        await sensorReadingRepository.InsertAsync(sensorReading);

        return Result.Created;
    }
}