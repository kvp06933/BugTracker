using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Extensions;
namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<BTUser> _userManager;

        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService, UserManager<BTUser> userManager)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the viewModel as a list
            List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();

            //Get company Id
            int companyId = User.Identity.GetCompanyId().Value;
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);
            //Loop over the users to populate the viewmodel

            // - instantialte ViewModel
            // -use _rolesServide
            //- create multi-selects
            foreach(BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new ManageUserRolesViewModel();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            //Return the model to the view
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            //Instantiate the BTUser
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);
            //Get Roles for the User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);
            //Grab the selected role
            string userRole = member.SelectedRoles.FirstOrDefault();
            //Remove User from their roles
            if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
            {
                

                if (!string.IsNullOrEmpty(userRole))
                {
                    //Add User to the new role
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }
                

            }

            //Navigate back to the View
            return RedirectToAction(nameof(ManageUserRoles));
            
        }
    }
}
