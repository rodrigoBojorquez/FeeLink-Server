using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;

public class ListLInkedUserWithToyQueryHandler(IUserRepository userRepository, IToyRepository toyRepository)
    : IRequestHandler<ListLinkedUsersWithToyQuery, ErrorOr<ListResult<UserResult>>>
{
    public async Task<ErrorOr<ListResult<UserResult>>> Handle(ListLinkedUsersWithToyQuery request,
        CancellationToken cancellationToken)
    {
        var toy = await toyRepository.GetByIdAsync(request.ToyId);

        if (toy is null)
            return Errors.Toy.NotFound;

        var users = await userRepository.ListByToyIdAsync(toy.Id, cancellationToken);

        return users;
    }
}