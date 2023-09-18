using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        private readonly ApplicationDbContext _dbContext;

        public BTCompanyInfoService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            try
            {
                List<BTUser> result = new List<BTUser>();
                result = await _dbContext.Users.Where(u => u.CompanyId == companyId).ToListAsync();
                return result;                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsAsync(int? companyId)
        {
            List<Project> result = new();
            try
            {
                if (companyId != null)
                {
                    result = await _dbContext.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                        .Include(c => c.Members)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.Comments)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.Attachments)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.History)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.Notifications)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.TicketType)
                        .Include(c => c.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                        .Include(c => c.ProjectPriority)
                        .ToListAsync();
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

            public async Task<List<Ticket>> GetAllTicketsAsync(int? companyId)
        {
            List<Ticket> result = new();
            List<Project> projects = new();

            try
            {
                projects = await GetAllProjectsAsync(companyId);
                result = projects.SelectMany(p => p.Tickets).ToList();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();
            try
            {
                if(companyId != null)
                {
                    result = await _dbContext.Companies
                        .Include(c => c.Members)
                        .Include(c => c.Project)
                        .Include(c => c.Invites)
                        .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
