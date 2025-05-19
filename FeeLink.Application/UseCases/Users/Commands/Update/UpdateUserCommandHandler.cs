using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Commands.Update;

public class UpdateUserCommandHandler(
    IRepository<User> repository,
    IAuthService userService,
    IPasswordService passwordService)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (!userService.HasSuperAccess())
            return Errors.Role.NotAllowed;

        var user = await repository.GetByIdAsync(command.Id);

        if (user is null)
            return Errors.User.NotFound;

        user.Name = command.Name;
        user.LastName = command.FirstLastName;
        user.Email = command.Email;
        user.Password = passwordService.HashPassword(command.Password);
        user.RoleId = command.RoleId;
        user.UpdateDate = DateTime.UtcNow;

        await repository.UpdateAsync(user);

        return Result.Updated;
    }
}