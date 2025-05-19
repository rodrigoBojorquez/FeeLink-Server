using FeeLink.Infrastructure.Common.Authentication;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.Interfaces.Services;
using FeeLink.Infrastructure.Common.Logging;
using FeeLink.Infrastructure.Data;
using FeeLink.Infrastructure.Data.Seeders;
using FeeLink.Infrastructure.Persistence;
using FeeLink.Infrastructure.Services.Assets;
using FeeLink.Infrastructure.Services.Authentication;
using FeeLink.Infrastructure.Services.Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeeLink.Infrastructure.Common.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config,
        IHostEnvironment env)
    {
        LoggingProfile.Configure(config);
        services.AddJWTAuthenticationScheme(config);
        // services.AddGoogleAuthenticationScheme(config);

        // Repositorios
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        
        // Servicios
        services.AddScoped<IPasswordService, PasswordService>();
        // services.AddScoped<IGovernmentApiService, GovernmentApiService>();
        services.AddScoped<ImageService>();
        services.AddScoped<ITokenService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        //Blob service
        
        // DbContext dependiendo del ambiente
        services.AddDbContext<FeeLinkDbContext>(opt =>
        {
            if (env.IsDevelopment())
            {
                opt.UseSqlite(config.GetConnectionString("FeeLinkConnection")).UseAsyncSeeding(
                    async (context, _, ct) =>
                    {
                        await Seeder.Administration.SeedAsync(context);
                    }).UseSeeding((context, _) =>
                {
                    Seeder.Administration.SeedAsync(context).Wait();
                });
            }
            else
            {
                opt.UseNpgsql(config.GetConnectionString("FeeLinkConnection")).UseAsyncSeeding(async (context, _, ct) =>
                {
                    await Seeder.Administration.SeedAsync(context);
                }).UseSeeding((context, _) =>
                {
                    Seeder.Administration.SeedAsync(context).Wait();
                });
            }
        });

        return services;
    }
}