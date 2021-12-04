﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Services.Interfaces;
using BugTracker.Extensions;
using BugTracker.Models.ViewModels;
using BugTracker.Models.Enums;

namespace BugTracker.Controllers
{
    #region prewritten code

    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;

        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTProjectService projectService, IBTLookupService lookupService, IBTRolesService rolesService, IBTFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _projectService = projectService;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> model = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            return View(model);
        }

        // GET: My Projects
        public async Task<IActionResult> MyProjects()
        {
            //Get Current User Id
            string userId = _userManager.GetUserId(User);
            List<Project> model = await _projectService.GetUserProjectsAsync(userId);

            return View(model);
        }

        // GET: All Projects
        public async Task<IActionResult> AllProjects()
        {
            //Get Current User Id
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> model = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            return View(model);
        }

        // GET: All Projects
        public async Task<IActionResult> ArchivedProjects()
        {
            //Get Current User Id
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> model = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);

            return View(model);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Remember that the _context should not be used directly in the controller so....     

            // Edit the following code to use the service layer. 
            // Your goal is to return the 'project' from the databse
            // with the Id equal to the parameter passed in.

            // This is the only modification necessary for this method/action.     

            int companyId = User.Identity.GetCompanyId().Value;

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            //Get company Id
            int companyId = User.Identity.GetCompanyId().Value;
            //Add View Model instance
            AddProjectWithPMViewModel model = new();
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");
            model.Priority = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.Image != null)
                    {
                        model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.Image);
                        model.Project.ImageName = model.Project.Image.FileName;
                        model.Project.ImageType = model.Project.Image.ContentType;
                    }
                    model.Project.CompanyId = companyId;
                    model.Project.Created = DateTimeOffset.Now;
                    await _projectService.AddNewProjectAsync(model.Project);
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    throw;
                }

            }


            ViewData["ProjectPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Id", model.Project.ProjectPriorityId);
            return View(model.Project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Get company Id
            int companyId = User.Identity.GetCompanyId().Value;
            //Add View Model instance
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");
            model.Priority = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            if (model.Project == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {

                try
                {
                    if (model.Project.Image != null)
                    {
                        model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.Image);
                        model.Project.ImageName = model.Project.Image.FileName;
                        model.Project.ImageType = model.Project.Image.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }


            ViewData["ProjectPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Id", model.Project.ProjectPriorityId);
            return View(model.Project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }
    }
    #endregion

    #region rewritten code
    //public class ProjectsController : Controller
    //{
    //    private readonly ApplicationDbContext _context;
    //    private readonly UserManager<BTUser> _userManager;
    //    private readonly IBTLookupService _lookupService;
    //    private readonly IBTProjectService _projectService;
    //    private readonly IBTRolesService _rolesService;
    //    private readonly IBTFileService _fileService;
    //    public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTLookupService lookupService, IBTProjectService projectService, IBTRolesService rolesService, IBTFileService fileService)
    //    {
    //        _context = context;
    //        _userManager = userManager;
    //        _lookupService = lookupService;
    //        _projectService = projectService;
    //        _rolesService = rolesService;
    //        _fileService = fileService;
    //    }

    //    // GET: Projects
    //    public async Task<IActionResult> Index()
    //    {
    //        int companyId = User.Identity.GetCompanyId().Value;
    //        var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority).Where(p => p.CompanyId == companyId);
    //        return View(await applicationDbContext.ToListAsync());
    //    }



    //    // GET: Projects/Details/5
    //    public async Task<IActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        // Remember that the _context should not be used directly in the controller so....     

    //        // Edit the following code to use the service layer. 
    //        // Your goal is to return the 'project' from the databse
    //        // with the Id equal to the parameter passed in.

    //        // This is the only modification necessary for this method/action.     

    //        int companyId = User.Identity.GetCompanyId().Value;

    //        Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

    //        if (project == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(project);
    //    }

    //    // GET: Projects/Create
    //    public async Task<IActionResult> Create()
    //    {
    //        //Get company Id
    //        int companyId = User.Identity.GetCompanyId().Value;
    //        //Add View Model instance
    //        AddProjectWithPMViewModel model = new();
    //        model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");
    //        model.Priority = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
    //        return View(model);
    //    }

    //    // POST: Projects/Create
    //    // To protect from overposting attacks, enable the specific properties you want to bind to.
    //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
    //    {
    //        if (model != null)
    //        {
    //            int companyId = User.Identity.GetCompanyId().Value;
    //            try
    //            {
    //                if (model.Project.Image != null)
    //                {
    //                    model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.Image);
    //                    model.Project.ImageName = model.Project.Image.FileName;
    //                    model.Project.ImageType = model.Project.Image.ContentType;
    //                }
    //                model.Project.CompanyId = companyId;
    //                model.Project.Created = DateTimeOffset.Now;
    //                await _projectService.AddNewProjectAsync(model.Project);
    //                if (!string.IsNullOrEmpty(model.PmId))
    //                {
    //                    await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
    //                }
    //                return RedirectToAction(nameof(Index));
    //            }
    //            catch (Exception)
    //            {

    //                throw;
    //            }

    //        }


    //        ViewData["ProjectPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Id", model.Project.ProjectPriorityId);
    //        return View(model.Project);
    //    }

    //    // GET: Projects/Edit/5
    //    public async Task<IActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        //Get company Id
    //        int companyId = User.Identity.GetCompanyId().Value;
    //        //Add View Model instance
    //        AddProjectWithPMViewModel model = new();

    //        model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
    //        model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");
    //        model.Priority = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

    //        if (model.Project == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(model);
    //    }

    //    // POST: Projects/Edit/5
    //    // To protect from overposting attacks, enable the specific properties you want to bind to.
    //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
    //    {
    //        if (model != null)
    //        {

    //            try
    //            {
    //                if (model.Project.Image != null)
    //                {
    //                    model.Project.ImageData = await _fileService.ConvertFileToByteArrayAsync(model.Project.Image);
    //                    model.Project.ImageName = model.Project.Image.FileName;
    //                    model.Project.ImageType = model.Project.Image.ContentType;
    //                }

    //                await _projectService.UpdateProjectAsync(model.Project);
    //                if (!string.IsNullOrEmpty(model.PmId))
    //                {
    //                    await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
    //                }
    //                return RedirectToAction("Index");
    //            }
    //            catch (DbUpdateConcurrencyException)
    //            {

    //                if (!await ProjectExists(model.Project.Id))
    //                {
    //                    return NotFound();
    //                }
    //                else
    //                {
    //                    throw;
    //                }
    //            }

    //        }


    //        ViewData["ProjectPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Id", model.Project.ProjectPriorityId);
    //        return View(model.Project);
    //    }

    //    // GET: Projects/Delete/5
    //    public async Task<IActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return NotFound();
    //        }

    //        var project = await _context.Projects
    //            .Include(p => p.Company)
    //            .Include(p => p.ProjectPriority)
    //            .FirstOrDefaultAsync(m => m.Id == id);
    //        if (project == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(project);
    //    }

    //    // POST: Projects/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<IActionResult> DeleteConfirmed(int id)
    //    {
    //        var project = await _context.Projects.FindAsync(id);
    //        _context.Projects.Remove(project);
    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }

    //    private async Task<bool> ProjectExists(int id)
    //    {
    //        int companyId = User.Identity.GetCompanyId().Value;
    //        return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
    //    }
    //}

    #endregion
}
