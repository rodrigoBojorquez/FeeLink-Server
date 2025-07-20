using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Repositories;

public interface IToyRepository : IRepository<Toy>
{
    Task<ListResult<ToyResult>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Toy?> GetByMacAsync(string macAddress, CancellationToken cancellationToken = default);
    
    Task<ListResult<Toy>> ListAsync(
        int page,
        int pageSize,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
    
    Task<Toy?> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
}