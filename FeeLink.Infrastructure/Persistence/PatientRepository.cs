using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Data;

namespace FeeLink.Infrastructure.Persistence;

public class PatientRepository(FeeLinkDbContext context) : GenericRepository<Patient>(context), IPatientRepository;