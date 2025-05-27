using System.Linq.Expressions;
using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Users.Common;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class UserRepository(FeeLinkDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        return Context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ListResult<User>> ListWithRoleAsync(int page = 1, int pageSize = 10,
        Expression<Func<User, bool>>? filter = null)
    {
        var query = Context.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new ListResult<User>(Page: page, PageSize: pageSize, TotalItems: totalItems,
            TotalPages: (int)Math.Ceiling((double)totalItems / pageSize), Items: items);
    }

    public async Task<User?> IncludeRoleAsync(Guid id)
    {
        return await Context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetByRecoveryTokenAsync(string recoveryToken)
    {
        throw new NotImplementedException();
    }

    public Task<ListResult<User>> ListAsync(int page = 1, int pageSize = 10, string? name = null,
        Guid? institutionId = null, Guid? roleId = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ListResult<UserResult>> ListByToyIdAsync(Guid toyId,
        CancellationToken cancellationToken = default)
    {
        var patientId = await Context.Toys
            .Where(t => t.Id == toyId)
            .Select(t => t.PatientId)
            .FirstOrDefaultAsync(cancellationToken);

        if (patientId == Guid.Empty)
            return new ListResult<UserResult>([], 0);

        var users = await Context.Users
            .Where(u =>
                u.TherapistAssignments.Any(ta => ta.PatientId == patientId) ||
                u.TutorAssignments.Any(tu => tu.PatientId == patientId)
            )
            .ToListAsync(cancellationToken);

        return new ListResult<UserResult>(
            Items: users.Select(u => u.ToResult()).ToList(),
            TotalItems: users.Count
        );
    }

    public new Task<ListResult<User>> ListAllAsync()
    {
        throw new NotImplementedException();
    }

    public new Task<ListResult<User>> ListAsync(int page = 1, int pageSize = 10,
        Expression<Func<User, bool>>? filter = null)
    {
        throw new NotImplementedException();
    }
}