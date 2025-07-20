using FeeLink.Application.Common.Extensions;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class PatientRepository(FeeLinkDbContext context) : GenericRepository<Patient>(context), IPatientRepository
{
    private readonly FeeLinkDbContext _context = context;

    public async Task<ListResult<Patient>> ListByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var data = await _context.Patients
            .Where(p => p.TherapistAssignments.Any(ta => ta.UserId == userId) ||
                        p.TutorAssignments.Any(ta => ta.UserId == userId))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.Patients
            .Where(p => p.TherapistAssignments.Any(ta => ta.UserId == userId) ||
                        p.TutorAssignments.Any(ta => ta.UserId == userId))
            .CountAsync(cancellationToken);

        return new ListResult<Patient>(Items: data, TotalItems: totalCount, Page: page, PageSize: pageSize,
            TotalPages: totalCount.GetTotalPages(pageSize));
    }

    public async Task<Patient?> IncludeAssignmentsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.Include(p => p.TherapistAssignments).Include(p => p.TutorAssignments)
            .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken: cancellationToken);
    }

    public async Task AssignTherapistsAsync(Guid patientId, List<Guid> therapistIds,
        CancellationToken cancellationToken = default)
    {
        _context.TherapistAssignments
            .RemoveRange(_context.TherapistAssignments.Where(ta => ta.PatientId == patientId));

        var assignments = therapistIds.Select(therapistId => new TherapistAssignment
        {
            PatientId = patientId,
            UserId = therapistId
        }).ToList();

        await _context.TherapistAssignments.AddRangeAsync(assignments, cancellationToken);
    }

    public async Task AssignTutorsAsync(Guid patientId, List<Guid> tutorIds,
        CancellationToken cancellationToken = default)
    {
        _context.TutorAssignments
            .RemoveRange(_context.TutorAssignments.Where(ta => ta.PatientId == patientId));

        var assignments = tutorIds.Select(tutorId => new TutorAssignment
        {
            PatientId = patientId,
            UserId = tutorId
        }).ToList();

        await _context.TutorAssignments.AddRangeAsync(assignments, cancellationToken);
    }

    public async Task<ListResult<Patient>> ListAsync(int page = 1, int pageSize = 10, string? search = null,
        Guid? therapistId = null, Guid? tutorId = null,
        CancellationToken cancellationToken = default)
    {
        var data = await _context.Patients
            .Where(p => (string.IsNullOrEmpty(search) || p.Name.Contains(search) || p.LastName.Contains(search)) &&
                        (!therapistId.HasValue || p.TherapistAssignments.Any(ta => ta.UserId == therapistId)) &&
                        (!tutorId.HasValue || p.TutorAssignments.Any(ta => ta.UserId == tutorId)))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.Patients
            .Where(p => (string.IsNullOrEmpty(search) || p.Name.Contains(search) || p.LastName.Contains(search)) &&
                        (!therapistId.HasValue || p.TherapistAssignments.Any(ta => ta.UserId == therapistId)) &&
                        (!tutorId.HasValue || p.TutorAssignments.Any(ta => ta.UserId == tutorId)))
            .CountAsync(cancellationToken);

        return new ListResult<Patient>(Items: data, TotalItems: totalCount, Page: page, PageSize: pageSize,
            TotalPages: totalCount.GetTotalPages(pageSize));
    }

    public async Task<bool> HasToyAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _context.Patients
            .Where(p => p.Id == patientId)
            .AnyAsync(p => p.Toy != null, cancellationToken);
    }

}