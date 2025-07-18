using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Roles.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Roles.Queries;

public record ListRolesQuery() : IRequest<ListResult<RoleResult>>;