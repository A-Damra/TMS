using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMS.Data;
using TMS.Models.PasswordModels;
using TMS.Models.UserModels;
using TMS.Services;

namespace TMS.Controllers
{
    public class PasswordController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TMSDbContext tMSDbContext;
        private readonly IEmailService emailService;

        public PasswordController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            TMSDbContext tMSDbContext, IEmailService emailService,
             SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tMSDbContext = tMSDbContext;
            this.emailService = emailService;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult ConfirmAndSetPassword(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest();

            var model = new ConfirmAndSetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAndSetPassword(ConfirmAndSetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            var emailResult = await userManager.ConfirmEmailAsync(user, model.Token);
            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            var passwordResult = await userManager.AddPasswordAsync(user, model.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            return RedirectToAction("Login", "Password");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);


            var ResetLink = Url.Action("SetPassword", "Password", new { userId = user.Id, token }, Request.Scheme);

            var message = $"Reset your Password by clicking this link: <a href='{ResetLink}'>link</a>";

            await emailService.SendEmail("Reset your Password", model.Email, message);

            return View("ResetPasswordConfirmation");

        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SetPassword(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest();
            var model = new SetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid user.");
                return View(model);
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }
            return RedirectToAction("Login", "Password");
        }
    }
}
