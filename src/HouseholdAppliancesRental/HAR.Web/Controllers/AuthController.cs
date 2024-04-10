using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Service.Contracts;
using HAR.Service.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            if (this.User.Identity!.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var model = new SignInViewModel();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.authService.SeedAdminUserAsync();

            var response = await this.authService.AuthenticateUserAsync(this.HttpContext, model);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("SignInErrors", response.Message!);
                return this.View(model);
            }

            return this.RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (this.User.Identity!.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var model = new SignUpViewModel()
            {
                Role = UserRole.User
            };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            if (await this.authService.UserExistsAsync(model.Email))
            {
                this.ModelState.AddModelError("SignUpErrors", "Имейлът е вече в употреба.");
                return this.View(model);
            }

            Response response = await this.authService.CreateUserAsync(model);

            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("SignUpErrors", response.Message!);
                return this.View(model);
            }

            return this.RedirectToAction("SignIn", "Auth");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SignOutAsync()
        {
            await this.HttpContext.SignOutAsync();
            return this.RedirectToAction("Index", "Home");
        }
    }
}
