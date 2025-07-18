using System.Linq.Expressions;
using ErrorOr;
using FeeLink.Application.Common.Extensions;
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
        return Context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.RecoveryToken == recoveryToken);
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

    public async Task<UserDataResult?> GetDataAsync(Guid tutorId, CancellationToken ct)
    {
        return await Context.Users
            .AsNoTracking()
            .Where(u => u.Id == tutorId)
            .Select(u => new UserDataResult(
                // 1) Datos básicos del tutor
                u.Id,
                u.Company.Name,
                u.Company.Address,

                // 2) Nombre del terapeuta del primer paciente de este tutor:
                u.TutorAssignments
                    // de cada asignación de tutor, trae al paciente
                    .Select(taTutor => taTutor.Patient)
                    // de cada paciente, a sus asignaciones de terapeuta
                    .SelectMany(p => p.TherapistAssignments)
                    // de cada asignación de terapeuta, al user que ejerce como terapeuta
                    .Select(taTher => taTher.User.Name)
                    .FirstOrDefault(),

                // 3) Datos del paciente
                u.TutorAssignments
                    .Select(ta => ta.Patient.Name)
                    .FirstOrDefault(),

                u.TutorAssignments
                    .Select(ta => (int?)ta.Patient.Age)
                    .FirstOrDefault(),

                u.TutorAssignments
                    .Select(ta => (Guid?)ta.Patient.Id)
                    .FirstOrDefault()
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ListResult<User>> ListAsync(int page, int pageSize, string? search = null, Guid? roleId = null)
    {
        var query = Context.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.Name.Contains(search) || u.Email.Contains(search));
        }

        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
        }

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new ListResult<User>(Page: page, PageSize: pageSize, TotalItems: totalItems,
            TotalPages: totalItems.GetTotalPages(pageSize), Items: items);
    }
}