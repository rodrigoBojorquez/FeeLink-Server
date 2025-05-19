using ErrorOr;
using FeeLink.Application.UseCases.Authentication.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.PasswordReset;

public record PasswordResetCommand(string RecoveryToken, string Password, string ConfirmPassword)
    : IRequest<ErrorOr<AuthResult>>;