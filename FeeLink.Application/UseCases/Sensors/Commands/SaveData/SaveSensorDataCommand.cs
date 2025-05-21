using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Sensors.Commands.SaveData;

public record SaveSensorDataCommand() : IRequest<ErrorOr<Created>>;