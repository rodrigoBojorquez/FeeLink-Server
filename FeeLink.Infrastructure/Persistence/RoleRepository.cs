using System.Linq.Expressions;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class RoleRepository(FeeLinkDbContext context) : GenericRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await Context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public new Task<ListResult<Role>> ListAllAsync()
    {
        throw new NotImplementedException();
    }

    public new Task<ListResult<Role>> ListAsync(int page = 1, int pageSize = 10, Expression<Func<Role, bool>>? filter = null)
    {
        throw new NotImplementedException();
    }
}