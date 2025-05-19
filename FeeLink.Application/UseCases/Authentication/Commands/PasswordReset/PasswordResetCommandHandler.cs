using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.PasswordReset;

public class PasswordResetCommandHandler(
    IUserRepository userRepository,
    IPasswordService passwordService,
    ITokenService tokenService)
    : IRequestHandler<PasswordResetCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(PasswordResetCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByRecoveryTokenAsync(command.RecoveryToken);

        if (user is null)
            return Errors.Authentication.InvalidToken;

        // Verificar que la nueva contraseña y su confirmación coincidan
        if (command.Password != command.ConfirmPassword)
            return Errors.Authentication.InvalidCredentials;

        // Actualizar la contraseña del usuario
        user.Password = passwordService.HashPassword(command.Password);
        user.RecoveryToken = null; // Invalidar el token

        await userRepository.UpdateAsync(user);

        var token = await tokenService.GenerateTokenAsync(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.StoreRefreshTokenAsync(refreshToken, user.Id);

        return new AuthResult(user.Id, token, refreshToken, user.Email,
            !string.IsNullOrEmpty(user.GoogleId), user.Name,
            user.LastName);
    }
}