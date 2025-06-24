using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Constants;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITokenService tokenService,
    IPasswordService passwordService)
    : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Para evitar problemas con el case sensitive de los emails
        var email = command.Email.ToLower();

        var user = await userRepository.GetByEmailAsync(email);

        if (user is not null)
            return Errors.User.DuplicateEmail;

        var devRole = await roleRepository.GetByNameAsync(FeeLinkConstants.UserBaseRole);

        if (devRole is null)
            return Errors.Role.NotFound;

        var newUser = new User
        {
            Name = command.Name,
            Email = email,
            Password = passwordService.HashPassword(command.Password),
            RoleId = devRole.Id
        };

        await userRepository.InsertAsync(newUser);

        var token = await tokenService.GenerateTokenAsync(newUser);
        var refreshToken = tokenService.GenerateRefreshToken();
        await tokenService.StoreRefreshTokenAsync(refreshToken, newUser.Id);

        return new AuthResult(
            newUser.Id,
            token,
            refreshToken,
            newUser.Email,
            newUser.Name
        );
    }
}