using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        #region prewritten code

        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        #endregion

        #region Constructor
        public BTProjectService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        #endregion

        #region Add New Project
        // CRUD - Create
        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Add Project Manager
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser currentPM = await GetProjectManagerAsync(projectId);

            // Remove the current PM if necessary
            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
                    return false;
                }
            }

            // Add the new PM
            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new PM. - Error: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Add User To Project
        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if (!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        #endregion

        #region Archive Project
        // CRUD - Archive (Delete)
        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                //Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Get All Project Members Except Project Manager
        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            try
            {
                List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, BTRoles.Developer.ToString());
                List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, BTRoles.Submitter.ToString());
                List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, BTRoles.Admin.ToString());

                List<BTUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

                return teamMembers;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Get All Projects By Company Id
        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();
                return projects;
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region Get All Projects By Priority
        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            try
            {
                List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
                int priorityId = await LookupProjectPriorityId(priorityName);

                return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Archived Projects
        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                                    .Include(p => p.Members)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Comments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Attachments)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.History)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.Notifications)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.ProjectPriority)
                                                    .ToListAsync();

                return projects;

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Developers On Project
        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get Project By Id
        // CRUD - Read
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                //Project project = await _context.Projects
                //                        .Include(p => p.Tickets)
                //                        .Include(p => p.Members)
                //                        .Include(p => p.ProjectPriority)
                //                        .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);


                Project project = await _context.Projects
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.OwnerUser)
                                                .Include(p => p.Members)
                                                .Include(p => p.ProjectPriority)
                                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                return project;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Get Project Manager
        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project project = await _context.Projects
                                        .Include(p => p.Members)
                                        .FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, BTRoles.ProjectManager.ToString()))
                    {
                        return member;
                    }
                }

                return null;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Get Project Mambers By Role
        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            try
            {
                Project project = await _context.Projects
                                        .Include(p => p.Members)
                                        .FirstOrDefaultAsync(p => p.Id == projectId);

                List<BTUser> members = new();

                foreach (var user in project.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(user, role))
                    {
                        members.Add(user);
                    }
                }

                return members;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Get Submitters On Project
        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get Unassigned Projects
        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> result = new();
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects
                                         .Include(p => p.ProjectPriority)
                                         .Where(p => p.CompanyId == companyId).ToListAsync();

                foreach (Project project in projects)
                {
                    if ((await GetProjectMembersByRoleAsync(project.Id, nameof(BTRoles.ProjectManager))).Count == 0)
                    {
                        result.Add(project);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        #endregion

        #region Get User Projects
        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Getting user projects list.  --> {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Get Users Not On Project
        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            try
            {
                List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

                return users.Where(u => u.CompanyId == companyId).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Is Assigned Project Manager

        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;

                if (projectManagerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Is User On Project

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                bool result = false;

                if (project != null)
                {
                    result = project.Members.Any(m => m.Id == userId);
                }

                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Look up Project Priority Id
        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            try
            {
                int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
                return priorityId;

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Remove Project Manager
        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project project = await _context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);
                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, BTRoles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Remove User From Project
        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Error Removing User from project.  --->  {ex.Message}");
            }
        }

        #endregion

        #region Remove Users from Project By Role
        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser btUser in members)
                {
                    try
                    {
                        project.Members.Remove(btUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Removing users from project.  --> {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Restore Project
        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                await UpdateProjectAsync(project);

                //Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }






        #endregion

        #region Update Project
        // CRUD - Update
        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion
        #region rewritten code

        //private readonly ApplicationDbContext _dbContext;
        //private readonly IBTRolesService _rolesService;
        //public BTProjectService(ApplicationDbContext dbContext, IBTRolesService rolesService)
        //{
        //    _dbContext = dbContext;
        //    _rolesService = rolesService;
        //}



        //public async Task AddNewProjectAsync(Project project)
        //{
        //    try
        //    {
        //        _dbContext.Add(project);
        //        await _dbContext.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        //{

        //    _dbContext.Add(userId, projectId);
        //    await _dbContext.SaveChangesAsync();
        //}

        //public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task ArchiveProjectAsync(Project project)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<BTUser> GetProjectManagerAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<Project>> GetUserProjectsAsync(string userId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> LookupProjectPriorityId(string priorityName)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RemoveProjectManagerAsync(int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RemoveUserFromProjectAsync(string userId, int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task RestoreProjectAsync(Project project)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateProjectAsync(Project project)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}
