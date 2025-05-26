using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Repositories;

public interface IToyRepository : IRepository<Toy>
{
    Task<ListResult<ToyResult>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}