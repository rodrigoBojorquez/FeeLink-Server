using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Services.Authentication;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Data.Seeders;

public static partial class Seeder
{
    public static class Administration
    {
        /**
         * Seeder para la gestion de permisos y roles de la aplicación
         */
        public static async Task SeedAsync(DbContext context)
        {
            if (!await context.Set<Role>().AnyAsync() && !await context.Set<User>().AnyAsync())
            {
                // Roles del sistema
                var roles = new List<Role>
                {
                    new Role
                    {
                        Name = "SuperAdmin",
                        DisplayName = "Super Administrador",
                        Description = "Acceso total al sistema"
                    },
                    new Role
                    {
                        Name = "Therapist",
                        DisplayName = "Terapueuta",
                    },
                    new Role
                    {
                        Name = "ClinicAdmin",
                        DisplayName = "Administrador de clinica",
                    }
                };

                await context.Set<Role>().AddRangeAsync(roles);
                await context.SaveChangesAsync();
                
                // Creacion de la compañia
                var company = new Company
                {
                    Name = "Centro de Terapia Infantil 'Tilin'",
                    Address = "123 Calle Principal",
                    Rfc = "CEJ123456789",
                    PhoneNumber = "+52-555-123-4567",
                };

                await context.Set<Company>().AddAsync(company);
                await context.SaveChangesAsync();

                var rolesIds = roles.ToDictionary(r => r.Name, r => r.Id);

                // Asignacion de usuarios prinpales
                var users = new List<User>
                {
                    new User
                    {
                        Name = "Rodrigo",
                        LastName = "Bojorquez",
                        Email = "rbojorquez1620@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Andres ",
                        LastName = "Garcia",
                        Email = "andres@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Andrea",
                        LastName = "Gomez",
                        Email = "andrea@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Diego",
                        LastName = "Aleman",
                        Email = "diego@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Fernando",
                        LastName = "Lopez",
                        Email = "fernando@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Michelle",
                        LastName = "Hernandez",
                        Email = "michelle@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Ricardo",
                        LastName = "Chi",
                        Email = "ricardo@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["SuperAdmin"]
                    },
                    new User
                    {
                        Name = "Joel",
                        LastName = "Vargas",
                        Email = "joel@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["Therapist"],
                        CompanyId = company.Id
                    },
                    new User
                    {
                        Name = "Renata",
                        LastName = "Mancilla",
                        Email = "renata@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["ClinicAdmin"],
                        CompanyId = company.Id
                    }
                };

                var passwordService = new PasswordService();

                foreach (var user in users)
                {
                    user.Password = passwordService.HashPassword(user.Password!);
                }

                await context.Set<User>().AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}