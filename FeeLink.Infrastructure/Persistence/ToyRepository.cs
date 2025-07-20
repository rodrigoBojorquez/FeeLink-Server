using FeeLink.Application.Common.Extensions;
using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class ToyRepository(FeeLinkDbContext context) : GenericRepository<Toy>(context), IToyRepository
{
    public async Task<ListResult<ToyResult>> ListByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var toys = await Context.Toys
            .Where(toy =>
                Context.TherapistAssignments.Any(ta => ta.UserId == userId && ta.PatientId == toy.PatientId) ||
                Context.TutorAssignments.Any(tu => tu.UserId == userId && tu.PatientId == toy.PatientId)
            )
            .ToListAsync(cancellationToken);

        return new ListResult<ToyResult>(
            Items: toys.Select(t => t.ToResult()).ToList(),
            TotalItems: toys.Count
        );
    }


    public async Task<Toy?> GetByMacAsync(string macAddress, CancellationToken cancellationToken = default)
    {
        return await Context.Toys
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.MacAddress == macAddress, cancellationToken);
    }

    public async Task<ListResult<Toy>> ListAsync(int page, int pageSize, Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Toy> query = Context.Toys;

        if (userId.HasValue)
        {
            query = query.Where(toy =>
                Context.TherapistAssignments.Any(ta => ta.UserId == userId && ta.PatientId == toy.PatientId) ||
                Context.TutorAssignments.Any(tu => tu.UserId == userId && tu.PatientId == toy.PatientId)
            );
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new ListResult<Toy>(Items: items, Page: page, PageSize: pageSize, TotalItems: totalItems,
            TotalPages: totalItems.GetTotalPages(pageSize));
    }

    public async Task<Toy?> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await Context.Toys
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.PatientId == patientId, cancellationToken);
    }
}