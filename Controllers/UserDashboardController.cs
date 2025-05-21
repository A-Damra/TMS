using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models.UserModels;
using TMS.Services;

namespace TMS.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TMSDbContext tMSDbContext;

        public UserDashboardController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, TMSDbContext tMSDbContext, IEmailService emailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tMSDbContext = tMSDbContext;
        }
        public async Task<IActionResult> UserDash()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var assignments = await tMSDbContext.assignments
                .Include(a => a.Project)
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            var grouped = assignments
                .GroupBy(a => a.Project)
                .Select(g => new ProjectAssignments
                {
                    ProjectName = g.Key.ProjectName,
                    Assignments = g.Select(a => new AssignmentInfo
                    {
                        AssignmentName = a.Name,
                        Description = a.Description,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate
                    }).ToList()
                }).ToList();

            var model = new UserDashboardViewModel
            {
                UserName = user.FirstName + " " + user.LastName,
                ProjectAssignments = grouped
            };

            return View(model);
        }

    }
}
