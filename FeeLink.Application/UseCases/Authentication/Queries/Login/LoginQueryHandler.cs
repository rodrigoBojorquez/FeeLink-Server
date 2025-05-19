using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public LoginQueryHandler(IUserRepository userRepository, ITokenService tokenService,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<AuthResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(query.Email);

        if (user is null)
            return Errors.Authentication.InvalidCredentials;

        if (user.GoogleId is not null)
            return Errors.Authentication.InvalidCredentials;

        if (!_passwordService.VerifyPassword(query.Password, user.Password!))
            return Errors.Authentication.InvalidCredentials;

        var token = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        await _tokenService.StoreRefreshTokenAsync(refreshToken, user.Id);

        return new AuthResult(user.Id, token, refreshToken, user.Email, false,
            user.Name, user.LastName);
    }
}