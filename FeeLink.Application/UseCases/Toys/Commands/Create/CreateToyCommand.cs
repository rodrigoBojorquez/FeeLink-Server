using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Toys.Commands.Create;

public record CreateToyCommand(
    Guid PatientId,
    string Name,
    string MacAddress) : IRequest<ErrorOr<Created>>;