using ErrorOr;
using FeeLink.Application.UseCases.Roles.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Roles.Queries.Get;

public record GetRoleQuery(Guid Id) : IRequest<ErrorOr<RoleResult>>;