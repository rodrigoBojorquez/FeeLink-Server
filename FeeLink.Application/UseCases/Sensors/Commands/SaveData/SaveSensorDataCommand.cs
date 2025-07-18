using ErrorOr;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Sensors.Commands.SaveData;

public record SaveSensorDataCommand(Guid ToyId, float Value, Metric Metric) : IRequest<ErrorOr<Created>>;