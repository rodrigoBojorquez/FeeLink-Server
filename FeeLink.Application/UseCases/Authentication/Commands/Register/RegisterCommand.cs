using ErrorOr;
using FeeLink.Application.UseCases.Authentication.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.Register;

public record RegisterCommand(
    string Name,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthResult>>;