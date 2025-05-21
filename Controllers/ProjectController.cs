using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;
using TMS.ViewModels;

namespace TMS.Controllers
{
    public class ProjectController : Controller
    {

        private readonly TMSDbContext tmsDbContext;
        public ProjectController(TMSDbContext tmsDbContext)
        {
            this.tmsDbContext = tmsDbContext;
        }

        public async Task<IActionResult> ProjectList()
        {
            var projects = await tmsDbContext.projects
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.User)
                .ToListAsync();

            return View(projects);
        }


        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(Project project)
        {
            if (!ModelState.IsValid)
                return View(project);

            await tmsDbContext.projects.AddAsync(project);
            await tmsDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ProjectList));
        }

        [HttpGet]
        public async Task<IActionResult> EditProject(Guid id)
        {
            var project = await tmsDbContext.projects.FindAsync(id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(Project project)
        {
            if (!ModelState.IsValid)
                return View(project);

            tmsDbContext.projects.Update(project);
            await tmsDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ProjectList));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await tmsDbContext.projects.FindAsync(id);
            if (project == null)
                return NotFound();

            return View(project);
        }

        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await tmsDbContext.projects.FindAsync(id);
            if (project == null)
                return NotFound();

            tmsDbContext.projects.Remove(project);
            await tmsDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(ProjectList));
        }

        [HttpGet]
        public async Task<IActionResult> AddAssignment(Guid id)
        {
            var users = await tmsDbContext.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FirstName + " " + u.LastName
                })
                .ToListAsync();

            var model = new AddAssignmentViewModel
            {
                ProjectId = id,
                Users = users
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAssignment(AddAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Users = await tmsDbContext.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.FirstName + " " + u.LastName
                    })
                    .ToListAsync();

                return View(model);
            }

            var assignment = new Assignment
            {
                AssignmentId = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ProjectId = model.ProjectId,
                UserId = model.UserId
            };

            await tmsDbContext.assignments.AddAsync(assignment);
            await tmsDbContext.SaveChangesAsync();

            return RedirectToAction("ProjectList");
        }


    }
}
