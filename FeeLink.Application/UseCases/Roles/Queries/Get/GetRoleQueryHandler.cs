using ErrorOr;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Roles.Common;
using FeeLink.Domain.Common.Errors;
using MediatR;

namespace FeeLink.Application.UseCases.Roles.Queries.Get;

public class GetRoleQueryHandler(IRoleRepository roleRepository) : IRequestHandler<GetRoleQuery, ErrorOr<RoleResult>>
{
    public async Task<ErrorOr<RoleResult>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id);
        if (role is null)
            return Errors.Role.NotFound;

        return role.ToResult();
    }
}