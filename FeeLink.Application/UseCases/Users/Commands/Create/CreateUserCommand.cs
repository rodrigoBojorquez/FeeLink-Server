using ErrorOr;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Commands.Create;

public record CreateUserCommand(
    string Name,
    string FirstLastName,
    string SecondLastName,
    string Email,
    string Password,
    Guid CompanyId,
    Guid RoleId) : IRequest<ErrorOr<Created>>;