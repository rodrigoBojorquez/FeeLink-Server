using FeeLink.Application.Common.Results;
using FeeLink.Application.UseCases.Toys.Common;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class ToyRepository(FeeLinkDbContext context) : GenericRepository<Toy>(context), IToyRepository
{
    public async Task<ListResult<ToyResult>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var toys = await Context.Patients
            // .Where(p => p.UserId == userId)
            .Select(p => p.Toy).ToListAsync(cancellationToken: cancellationToken);
        
        return new ListResult<ToyResult>(Items: toys.Select(t => t.ToResult()), toys.Count);
    }
}