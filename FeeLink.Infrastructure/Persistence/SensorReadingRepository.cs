using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;

namespace FeeLink.Infrastructure.Persistence;

public class SensorReadingRepository(FeeLinkDbContext context)
    : GenericRepository<SensorReading>(context), ISensorReadingRepository
{
}