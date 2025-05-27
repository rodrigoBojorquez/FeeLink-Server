using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Queries.Login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordService passwordService)
    : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
            return Errors.Authentication.InvalidCredentials;

        bool passwordIsValid = passwordService.VerifyPassword(request.Password, user.Password);

        if (!passwordIsValid)
            return Errors.Authentication.InvalidCredentials;

        var token = await tokenService.GenerateTokenAsync(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await tokenService.StoreRefreshTokenAsync(refreshToken, user.Id);

        return new AuthResult(
            user.Id, token, refreshToken, user.Email, user.Name, user.LastName, user.Picture);
    }
}