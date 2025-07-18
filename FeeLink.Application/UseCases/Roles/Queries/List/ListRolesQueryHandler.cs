using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Roles.Common;
using MediatR;

namespace FeeLink.Application.UseCases.Roles.Queries;

public class ListRolesQueryHandler(IRoleRepository roleRepository) : IRequestHandler<ListRolesQuery, ListResult<RoleResult>>
{
    public async Task<ListResult<RoleResult>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleRepository.ListAllAsync();
        var roleResults = roles.Items.Select(role => role.ToResult()).ToList();
        return ListResult<RoleResult>.From(roles, roleResults);
    }
}