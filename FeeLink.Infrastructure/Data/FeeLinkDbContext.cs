using System.Reflection;
using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Data;

public class FeeLinkDbContext : DbContext
{
    public FeeLinkDbContext(DbContextOptions<FeeLinkDbContext> options) : base(options)
    {
    }
    
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorType> SensorTypes { get; set; }
    public DbSet<SensorReading> SensorReadings { get; set; }
    public DbSet<Toy> Toys { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}