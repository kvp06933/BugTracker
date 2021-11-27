using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        public UserRolesController(IBTRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the viewModel
            //Get all company users

            //Loop over the users to populate the viewmodel
            // - instantialte ViewModel
            // -use _rolesServide
            //- create multi-selects
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles([ViewModel])
        {
            //Instantiate the BTUser
            //Get Roles for the User
            //Remove User from their roles

            //Grab the selected role

            //Add User to the new role
        }
    }
}
