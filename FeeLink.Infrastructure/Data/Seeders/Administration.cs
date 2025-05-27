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
                    },
                    new Role
                    {
                        Name = "Tutor",
                        DisplayName = "Tutor",
                        Description = "Acceso limitado a la información de los pacientes"
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
                    },
                    new User
                    {
                        Name = "Giovanni",
                        LastName = "Dzul",
                        Email = "tutor@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["Tutor"],
                        CompanyId = company.Id
                    },
                    new User
                    {
                        Name = "Eduardo",
                        LastName = "Rodriguez",
                        Email = "eduardo@gmail.com",
                        Password = "password",
                        RoleId = rolesIds["Tutor"],
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

                var userIds = users.ToDictionary(u => u.Email, u => u.Id);

                // Agregar pacientes 
                var patients = new List<Patient>
                {
                    new Patient
                    {
                        Name = "Fernando",
                        LastName = "Villafaña",
                        Age = 10,
                        Gender = "Masculino",
                        Height = 1.4f,
                        Weight = 35.0f,
                    },
                    new Patient
                    {
                        Name = "Sofia",
                        LastName = "Mendez",
                        Age = 8,
                        Gender = "Femenino",
                        Height = 1.2f,
                        Weight = 25.0f,
                    }
                };

                await context.Set<Patient>().AddRangeAsync(patients);
                await context.SaveChangesAsync();
                
                var patientIds = patients.ToDictionary(p => p.Name, p => p.Id);

                // Agregar juguetes
                var toys = new List<Toy>
                {
                    new Toy
                    {
                        Name = "Tilin",
                        PatientId = patients.First().Id,
                        MacAddress = "00:1A:2B:3C:4D:5A"
                    },
                    new Toy
                    {
                        Name = "Tralalero",
                        PatientId = patients.Last().Id,
                        MacAddress = "00:1A:2B:3C:4D:5B"
                    }
                };

                await context.Set<Toy>().AddRangeAsync(toys);
                await context.SaveChangesAsync();

                // Asignar terapeutas a los pacientes
                var patientTherapistAssignments = new List<TherapistAssignment>
                {
                    new TherapistAssignment
                    {
                        // Villafaña
                        PatientId = patientIds["Fernando"],
                        // Joel Vargas
                        UserId = userIds["joel@gmail.com"]
                    },
                    new TherapistAssignment
                    {
                        PatientId = patientIds["Sofia"],
                        UserId = userIds["joel@gmail.com"]
                    }
                };
                
                await context.Set<TherapistAssignment>().AddRangeAsync(patientTherapistAssignments);
                await context.SaveChangesAsync();
                
                // Asignar tutores a los pacientes
                var patientTutorAssignments = new List<TutorAssignment>
                {
                    new TutorAssignment
                    {
                        PatientId = patients[0].Id,
                        UserId = userIds.GetValueOrDefault("tutor@gmail.com")
                    },
                    new TutorAssignment
                    {
                        PatientId = patients[1].Id,
                        UserId = userIds.GetValueOrDefault("eduardo@gmail.com")
                    }
                };
                
                await context.Set<TutorAssignment>().AddRangeAsync(patientTutorAssignments);
                await context.SaveChangesAsync();
            }
        }
    }
}