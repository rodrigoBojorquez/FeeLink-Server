using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;

namespace FeeLink.Infrastructure.Persistence;

public class ToyRepository : GenericRepository<Toy> , IToyRepository
{
    public ToyRepository(FeeLinkDbContext context) : base(context)
    {
    }
}