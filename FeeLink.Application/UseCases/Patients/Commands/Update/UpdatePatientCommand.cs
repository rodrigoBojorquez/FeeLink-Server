using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Patients.Commands.Update;

public record UpdatePatientCommand(
    Guid Id,
    string Name,
    string LastName,
    int Age,
    string Gender,
    float Height,
    float Weight) : IRequest<ErrorOr<Updated>>;