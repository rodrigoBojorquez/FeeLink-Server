using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.Create;

public record CreatePatientCommand(
    string Name,
    string LastName,
    int Age,
    string Gender,
    float Height,
    float Weight,
    Guid UserId)
    : IRequest<ErrorOr<Created>>;