using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        //A constructor allows us to instantiate a class.
        public BTRolesService(RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager, ApplicationDbContext dbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            try
            {
                bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
                return result;
            }
            catch(System.Exception)
            {
                throw;
            }
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            try
            {
                IdentityRole role = _dbContext.Roles.Find(roleId);
                string result =await _roleManager.GetRoleNameAsync(role);
                return result;
            }
            catch(System.Exception)
            {
                throw;
            }
        }

        public Task<List<IdentityRole>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}
