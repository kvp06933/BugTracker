using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        #region prewritten code

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

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {

                List<IdentityRole> result = new List<IdentityRole>();
                result = await _dbContext.Roles.ToListAsync();
                return result;
            }
            catch(System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            try
            {
                IEnumerable<string> result = await _userManager.GetRolesAsync(user);
                return result;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            try
            {
                List<BTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
                List<BTUser> result = users.Where(u => u.CompanyId == companyId).ToList();

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            {
                throw new NotImplementedException();
            }

        }

        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            try
            {
                bool result = await _userManager.IsInRoleAsync(user, roleName);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            try
            {
                bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        

    }
}
