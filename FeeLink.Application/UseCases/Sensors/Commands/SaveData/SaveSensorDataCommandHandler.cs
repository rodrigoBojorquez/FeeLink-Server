using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Sensors.Commands.SaveData;

public class SaveSensorDataCommandHandler : IRequestHandler<SaveSensorDataCommand, ErrorOr<Created>>
{
    public Task<ErrorOr<Created>> Handle(SaveSensorDataCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}