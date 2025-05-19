using System.Linq.Expressions;
using FeeLink.Application.Common.Results;
using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<Domain.Entities.User?> GetByEmailAsync(string email);

    Task<ListResult<Domain.Entities.User>> ListWithRoleAsync(int page = 1, int pageSize = 10,
        Expression<Func<Domain.Entities.User, bool>>? filter = null);

    Task<Domain.Entities.User?> IncludeRoleAsync(Guid id);

    Task<User?> GetByRecoveryTokenAsync(string recoveryToken);

    Task<ListResult<User>> ListAsync(int page = 1, int pageSize = 10, string? name = null, Guid? institutionId = null,
        Guid? roleId = null);
}