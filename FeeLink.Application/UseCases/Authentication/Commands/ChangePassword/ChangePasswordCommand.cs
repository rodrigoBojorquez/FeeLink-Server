using ErrorOr;
using FeeLink.Application.UseCases.Authentication.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmPassword, string RefreshToken)
    : IRequest<ErrorOr<AuthResult>>;