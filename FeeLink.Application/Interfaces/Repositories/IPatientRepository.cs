using FeeLink.Application.Common.Results;
using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<List<Patient>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Patient?> IncludeAssignmentsAsync(Guid patientId, CancellationToken cancellationToken = default);
    
    Task AssignTherapistsAsync(Guid patientId, List<Guid> therapistIds, CancellationToken cancellationToken = default);
    
    Task AssignTutorsAsync(Guid patientId, List<Guid> tutorIds, CancellationToken cancellationToken = default);

    Task<ListResult<Patient>> ListAsync(int page = 1, int pageSize = 10, string? search = null, 
        Guid? therapistId = null, Guid? tutorId = null, CancellationToken cancellationToken = default);
}