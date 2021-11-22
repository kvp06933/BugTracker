using BugTracker.Models;
using BugTracker.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Data
{
    public static class DataUtility
    {
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();
            await dbContextSvc.Database.MigrateAsync();

            //Things we need to create:
            //Roles
            await SeedRolesAsync(roleManagerSvc);
            //Companies
            await SeedCompaniesAsync(dbContextSvc);
            //Users
            await SeedUsersAsync(userManagerSvc);
            //Demo Users
            await SeedDemoUsersAsync(userManagerSvc);
            //ProjectPriorities
            await SeedProjectPrioritiesAsync(dbContextSvc);
            //Ticket Statuses
            await SeedTicketStatusesAsync(dbContextSvc);
            //Ticket Priorities
            await SeedTicketPrioritiesAsync(dbContextSvc);
            //Ticket Types
            await SeedTicketTypesAsync(dbContextSvc);
            //Notification Types
            await SeedNotificationTypesAsync(dbContextSvc);
            //Projects
            await SeedProjectsAsync(dbContextSvc);
            //Tickets
            await SeedTicketsAsync(dbContextSvc);
        }
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManagerSvc)
        {
            await roleManagerSvc.CreateAsync(new IdentityRole(nameof(BTRoles.Admin)));
            await roleManagerSvc.CreateAsync(new IdentityRole(nameof(BTRoles.ProjectManager)));
            await roleManagerSvc.CreateAsync(new IdentityRole(nameof(BTRoles.Developer)));
            await roleManagerSvc.CreateAsync(new IdentityRole(nameof(BTRoles.Submitter)));
            await roleManagerSvc.CreateAsync(new IdentityRole(nameof(BTRoles.DemoUser)));
        }

        private static async Task SeedCompaniesAsync(ApplicationDbContext dbContextSvc)
        {
            try
            {
                IList<Company> defaultCompanies = new List<Company>()
                {
                    new Company() {Name = "Company 1", Description = "This is default Company 1"},
                    new Company() {Name = "Company 2", Description = "This is default Company 2"},
                    new Company() {Name = "Company 3", Description = "This is default Company 3"},
                    new Company() {Name = "Company 4", Description = "This is default Company 4"},
                    new Company() {Name = "Company 5", Description = "This is default Company 5"},
                };

                var dbCompanies = dbContextSvc.Companies.Select(c => c.Name).ToList();

                await dbContextSvc.Companies.AddRangeAsync(defaultCompanies.Where(c => !dbCompanies.Contains(c.Name)));
                await dbContextSvc.SaveChangesAsync();

                company1Id = dbContextSvc.Companies.FirstOrDefault(c => c.Name == "Company 1").Id;
                company2Id = dbContextSvc.Companies.FirstOrDefault(c => c.Name == "Company 2").Id;
                company3Id = dbContextSvc.Companies.FirstOrDefault(c => c.Name == "Company 3").Id;
                company4Id = dbContextSvc.Companies.FirstOrDefault(c => c.Name == "Company 4").Id;
                company5Id = dbContextSvc.Companies.FirstOrDefault(c => c.Name == "Company 5").Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Companies");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

        }

        private static async Task SeedUsersAsync(UserManager<BTUser> userManagerSvc)
        {
            var defaultUser = new BTUser
            {
                Email = "kpitts@mailinator.com",
                UserName = "kpitts@mailinator.com",
                FirstName = "Katherine",
                LastName = "Pitts",
                PhoneNumber = "555-1212",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Admin User 1");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "pparker@mailinator.com",
                UserName = "pparker@mailinator.com",
                FirstName = "Peter",
                LastName = "Parker",
                PhoneNumber = "555-1313",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Admin.ToString());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Admin User 2");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "tstark@mailinator.com",
                UserName = "tstark@mailinator.com",
                FirstName = "Tony",
                LastName = "Stark",
                PhoneNumber = "555-1414",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 1");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "bwayne@mailinator.com",
                UserName = "bwayne@mailinator.com",
                FirstName = "Bruce",
                LastName = "Wayne",
                PhoneNumber = "555-1515",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 2");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "bbanner@mailinator.com",
                UserName = "bbanner@mailinator.com",
                FirstName = "Bruce",
                LastName = "Banner",
                PhoneNumber = "555-1616",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 3");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "hmarks@mailinator.com",
                UserName = "hmarks@mailinator.com",
                FirstName = "Hannah",
                LastName = "Marks",
                PhoneNumber = "555-1717",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 4");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "skyle@mailinator.com",
                UserName = "skyle@mailinator.com",
                FirstName = "Selena",
                LastName = "Kyle",
                PhoneNumber = "555-1818",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 5");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "hquinn@mailinator.com",
                UserName = "hquinn@mailinator.com",
                FirstName = "Harley",
                LastName = "Quinn",
                PhoneNumber = "555-1919",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Developer User 6");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "ckringle@mailinator.com",
                UserName = "kcringle@mailinator.com",
                FirstName = "Chris",
                LastName = "Kringle",
                PhoneNumber = "555-2020",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 1");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "nstark@mailinator.com",
                UserName = "nstark@mailinator.com",
                FirstName = "Ned",
                LastName = "Stark",
                PhoneNumber = "555-2121",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 2");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "hpotter@mailinator.com",
                UserName = "hpotter@mailinator.com",
                FirstName = "Harry",
                LastName = "Potter",
                PhoneNumber = "555-2222",
                EmailConfirmed = true,
                CompanyId = company1Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 3");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "rweasley@mailinator.com",
                UserName = "rweasley@mailinator.com",
                FirstName = "Ron",
                LastName = "Weasley",
                PhoneNumber = "555-2323",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 4");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }

            defaultUser = new BTUser
            {
                Email = "hgranger@mailinator.com",
                UserName = "hgranger@mailinator.com",
                FirstName = "Hermione",
                LastName = "Granger",
                PhoneNumber = "555-2424",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 5");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
            defaultUser = new BTUser
            {
                Email = "omonroe@mailinator.com",
                UserName = "omonroe@mailinator.com",
                FirstName = "Ororo",
                LastName = "Monroe",
                PhoneNumber = "555-2525",
                EmailConfirmed = true,
                CompanyId = company2Id
            };
            try
            {
                var user = await userManagerSvc.FindByEmailAsync(defaultUser.Email);
                if (user is null)
                {
                    await userManagerSvc.CreateAsync(defaultUser, "Abc&123!");
                    await userManagerSvc.AddToRoleAsync(defaultUser, BTRoles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Submitter User 6");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
        }

        private static async Task SeedDemoUsersAsync(UserManager<BTUser> userManagerSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedProjectPrioritiesAsync(ApplicationDbContext dbContextSvc)
        {
            try
            {
                IList<ProjectPriority> defaultPriorities = new List<ProjectPriority>()
                {
                    new ProjectPriority() {Name = "Low" },
                    new ProjectPriority() {Name = "Medium"},
                    new ProjectPriority() {Name = "High"},
                    new ProjectPriority() {Name = "Urgent"},

                };

                var dbPriorities = dbContextSvc.ProjectPriorities.Select(c => c.Name).ToList();

                await dbContextSvc.ProjectPriorities.AddRangeAsync(defaultPriorities.Where(c => !dbPriorities.Contains(c.Name)));
                await dbContextSvc.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Project Priorities");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
        }

        private static async Task SeedTicketStatusesAsync(ApplicationDbContext dbContextSvc)
        {
            try
            {
                IList<TicketStatus> defaultTicketStatuses = new List<TicketStatus>()
                {
                    new TicketStatus() {Name = "New" },
                    new TicketStatus() {Name = "Development"},
                    new TicketStatus() {Name = "Testing"},
                    new TicketStatus() {Name = "Resolved"},

                };

                var dbTicketStatuses = dbContextSvc.TicketStatuses.Select(c => c.Name).ToList();

                await dbContextSvc.TicketStatuses.AddRangeAsync(defaultTicketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name)));
                await dbContextSvc.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Ticket Statuses");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
        }

        private static async Task SeedTicketPrioritiesAsync(ApplicationDbContext dbContextSvc)
        {
            try
            {
                IList<TicketPriority> defaultTicketPriorities = new List<TicketPriority>()
                {
                    new TicketPriority() {Name = "Low" },
                    new TicketPriority() {Name = "Medium"},
                    new TicketPriority() {Name = "High"},
                    new TicketPriority() {Name = "Urgent"},

                };

                var dbTicketPriorities = dbContextSvc.TicketPriorities.Select(c => c.Name).ToList();

                await dbContextSvc.TicketPriorities.AddRangeAsync(defaultTicketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name)));
                await dbContextSvc.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Ticket Statuses");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
        }

        private static async Task SeedTicketTypesAsync(ApplicationDbContext dbContextSvc)
        {
            try
            {
                IList<TicketType> defaultTicketTypes = new List<TicketType>()
                {
                    new TicketType() {Name = "New Development" },
                    new TicketType() {Name = "Work Task"},
                    new TicketType() {Name = "Defect"},
                    new TicketType() {Name = "Change Request"},
                    new TicketType() {Name = "Enhancement"},
                    new TicketType() {Name = "General Task"},

                };

                var dbTicketTypes = dbContextSvc.TicketTypes.Select(c => c.Name).ToList();

                await dbContextSvc.TicketTypes.AddRangeAsync(defaultTicketTypes.Where(c => !dbTicketTypes.Contains(c.Name)));
                await dbContextSvc.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Console.WriteLine("******** ERROR ********");
                Console.WriteLine("Error Seeding Ticket Types");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************");
            }
        }

        private static async Task SeedNotificationTypesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedProjectsAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedTicketsAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }
    }

}








    

