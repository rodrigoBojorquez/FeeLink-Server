using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Authentication.Common;
using FeeLink.Domain.Common.Constants;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<AuthResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Para evitar problemas con el case sensitive de los emails
        var email = command.Email.ToLower();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is not null)
            return Errors.User.DuplicateEmail;

        var devRole = await _roleRepository.GetByNameAsync(FeeLinkConstants.UserBaseRole);

        if (devRole is null)
            return Errors.Role.NotFound;

        var newUser = new User
        {
            Name = command.Name,
            LastName = command.LastName,
            Email = email,
            Password = _passwordService.HashPassword(command.Password),
            RoleId = devRole.Id
        };

        await _userRepository.InsertAsync(newUser);

        var token = await _tokenService.GenerateTokenAsync(newUser);
        var refreshToken = _tokenService.GenerateRefreshToken();
        await _tokenService.StoreRefreshTokenAsync(refreshToken, newUser.Id);

        return new AuthResult(
            newUser.Id,
            token,
            refreshToken,
            newUser.Email,
            false,
            newUser.Name,
            newUser.LastName
        );
    }
}