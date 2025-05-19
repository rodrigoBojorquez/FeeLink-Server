using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, ErrorOr<ListResult<UserResult>>>
{
    private readonly IUserRepository _repository;
    private readonly IAuthService _userService;

    public ListUsersQueryHandler(IUserRepository repository, IAuthService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<ErrorOr<ListResult<UserResult>>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var data = await _repository.ListWithRoleAsync(query.Page, query.PageSize,
            u => query.Name != null && u.Name.ToLower().Contains(query.Name.ToLower()));

        return ListResult<UserResult>.From(data, data.Items.Select(i => i.ToResult()));
    }
}