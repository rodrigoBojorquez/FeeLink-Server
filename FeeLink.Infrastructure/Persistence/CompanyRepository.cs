using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;

namespace FeeLink.Infrastructure.Persistence;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(FeeLinkDbContext context) : base(context)
    {
    }
}