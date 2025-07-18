using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.ListUsers;

public class ListUsersQueryHandler(IUserRepository repository)
    : IRequestHandler<ListUsersQuery, ErrorOr<ListResult<UserResult>>>
{

    public async Task<ErrorOr<ListResult<UserResult>>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var data = await repository.ListAsync(query.Page, query.PageSize, query.Name, query.RoleId);
        return ListResult<UserResult>.From(data, data.Items.Select(u => u.ToResult()));
    }
}