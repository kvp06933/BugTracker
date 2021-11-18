using BugTracker.Models;
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

        private static async Task SeedTicketsAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedProjectsAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedNotificationTypesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedTicketTypesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedTicketPrioritiesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedTicketStatusesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedProjectPrioritiesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedCompaniesAsync(ApplicationDbContext dbContextSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedUsersAsync(UserManager<BTUser> userManagerSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedDemoUsersAsync(UserManager<BTUser> userManagerSvc)
        {
            throw new NotImplementedException();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManagerSvc)
        {
            throw new NotImplementedException();
        }
    }
}
