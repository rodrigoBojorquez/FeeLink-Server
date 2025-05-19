using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Commands.Create;

public class CreateUserCommandHandler(
    IRepository<User> repository,
    IAuthService userService,
    IPasswordService passwordService)
    : IRequestHandler<CreateUserCommand, ErrorOr<Created>>
{
    public async Task<ErrorOr<Created>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!userService.HasSuperAccess())
            return Errors.Role.NotAllowed;

        var user = new User
        {
            Name = command.Name,
            LastName = command.FirstLastName,
            Email = command.Email,
            Password = command.Password,
            RoleId = command.RoleId,
        };

        user.Password = passwordService.HashPassword(user.Password);

        await repository.InsertAsync(user);

        return Result.Created;
    }
}