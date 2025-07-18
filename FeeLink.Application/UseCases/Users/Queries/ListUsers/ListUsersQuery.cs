using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Users.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Users.Queries.ListUsers;

public record ListUsersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    Guid? RoleId = null)
    : IRequest<ErrorOr<ListResult<UserResult>>>;