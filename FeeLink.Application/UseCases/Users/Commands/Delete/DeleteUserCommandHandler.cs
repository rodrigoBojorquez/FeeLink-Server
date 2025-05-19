using ErrorOr;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Commands.Delete;

public class DeleteUserCommandHandler(IRepository<User> repository, IAuthService userService)
    : IRequestHandler<DeleteUserCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        if (!userService.HasSuperAccess())
        {
            var userId = userService.GetUserId();
            if (userId != command.Id)
                return Errors.Authentication.NotAuthorized;
        }

        var user = await repository.GetByIdAsync(command.Id);

        if (user is null)
            return Errors.User.NotFound;

        user.IsDeleted = true;
        user.DeleteDate = DateTime.UtcNow;

        await repository.UpdateAsync(user);

        return Unit.Value;
    }
}