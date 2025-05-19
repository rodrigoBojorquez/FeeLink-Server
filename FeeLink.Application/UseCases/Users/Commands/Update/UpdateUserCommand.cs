using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Commands.Update;

public record UpdateUserCommand(
    Guid Id,
    string Name,
    string FirstLastName,
    string SecondLastName,
    string Email,
    string Password,
    Guid? EntityId,
    Guid RoleId) : IRequest<ErrorOr<Updated>>;