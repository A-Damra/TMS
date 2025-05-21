using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models.UserModels;
using TMS.Services;

namespace TMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TMSDbContext tMSDbContext;
        private readonly IEmailService emailService;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, TMSDbContext tMSDbContext, IEmailService emailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tMSDbContext = tMSDbContext;
            this.emailService = emailService;
        }

        public async Task<IActionResult> UserList()
        {
            var model = await userManager.Users
                .Include(u => u.Department)
                .ToListAsync();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentList();
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                DepartmentId = model.DepartmentId,
                IsAdmin = model.IsAdmin
            };

            var result = await userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Add user to role

                await RolesExist();

                await AssignRoleAsync(user, model.IsAdmin);

                await NewUserMail(user);

                return RedirectToAction("UserList", "Account");

            }

            AddErrors(result);

            await PopulateDepartmentList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null)
                return BadRequest();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Missing token");
            }


            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // redirect to password set
                return RedirectToAction("ConfirmAndSetPassword", "Password", new { userId = user.Id, token });
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("UserList", "Account");
            }
            else
            {
                AddErrors(result);
                return View("UserList", await userManager.Users.ToListAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UpdateUserViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                DepartmentId = user.DepartmentId,
                IsAdmin = user.IsAdmin
            };

            await PopulateDepartmentList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentList();
                return View(model);
            }
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.DepartmentId = model.DepartmentId;
            user.IsAdmin = model.IsAdmin;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("UserList", "Account");
            }
            else
            {
                AddErrors(result);
                await PopulateDepartmentList();

                return View(model);
            }
        }

        // Helper methods
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task RolesExist()
        {

            if (await roleManager.FindByNameAsync("User") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }

        private async Task AssignRoleAsync(User user, bool isAdmin)
        {
            string roleName = isAdmin ? "Admin" : "User";
            await userManager.AddToRoleAsync(user, roleName);
        }

        private async Task NewUserMail(User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);
            var email = user.Email;
            var message = $"Please confirm your email and set your password by clicking this link: <a href='{confirmationLink}'>Link</a>";

            await emailService.SendEmail("Confirm your email", email, message);
        }

        private async Task PopulateDepartmentList()
        {
            ViewBag.Departments = await tMSDbContext.departments
            .Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.Name
            }).ToListAsync();

        }


    }
}
