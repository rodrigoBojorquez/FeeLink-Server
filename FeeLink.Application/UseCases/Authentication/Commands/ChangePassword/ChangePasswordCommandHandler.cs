using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IAuthService userService,
    ITokenService tokenService)
    : IRequestHandler<ChangePasswordCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();

        if (userId.IsError)
            return userId.Errors;

        var user = await userRepository.IncludeRoleAsync(userId.Value);

        if (user is null)
            return Errors.User.NotFound;

        var isCurrentPasswordValid = passwordService.VerifyPassword(command.OldPassword, user.Password!);

        if (!isCurrentPasswordValid) return Errors.Authentication.InvalidCredentials;

        var newHashedPassword = passwordService.HashPassword(command.NewPassword);
        user.Password = newHashedPassword;

        await userRepository.UpdateAsync(user);

        var result = await tokenService.RefreshToken(command.RefreshToken);

        return result;
    }
}