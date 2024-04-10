using HAR.Common;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Web.Models.Rent;
using HAR.Web.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ICurrentUser currentUser;
        private readonly UserManager<User> userManager;
        private readonly IRentService rentService;

        public UserController(IUserService userService, ICurrentUser currentUser, UserManager<User> userManager, IRentService rentService)
        {
            this.userService = userService;
            this.currentUser = currentUser;
            this.userManager = userManager;
            this.rentService = rentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            User? user = await this.userManager.FindByIdAsync(this.currentUser.UserId!);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Rents(RentFilterQuery filterQuery)
        {
            var rents = await this.rentService.FetchCurrentUserRentsAsync(filterQuery);
            return this.View(rents);
        }

        [HttpGet]
        public async Task<IActionResult> EditName()
        {
            User? user = await this.userManager.FindByIdAsync(this.currentUser.UserId!);

            if (user == null)
            {
                return this.NotFound();
            }

            EditNameViewModel model = new EditNameViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditName(EditNameViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            User? user = await this.userManager.FindByIdAsync(this.currentUser.UserId!);

            if (user == null)
            {
                this.ModelState.AddModelError("EditNameErrors", "Потебителя не бе намерен.");
                return this.NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var response = await this.userManager.UpdateAsync(user);
            if (!response.Succeeded)
            {
                this.ModelState.AddModelError("EditNameErrors", response.Errors.First().ToString()!);
                return this.NotFound();
            }

            return this.RedirectToAction("Settings", "User");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            User? user = await this.userManager.FindByIdAsync(this.currentUser.UserId!);

            if (user == null)
            {
                this.ModelState.AddModelError("ChangePasswordErrors", "Потребителя не бе намерен.");
                return this.NotFound();
            }

            var checkPassResponse = await this.userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!checkPassResponse)
            {
                this.ModelState.AddModelError("ChangePasswordErrors", "Грешна текуща парола.");
                return this.View();
            }

            var response = await this.userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!response.Succeeded)
            {
                this.ModelState.AddModelError("ChangePasswordErrors", "Паролата не успя да се обнови.");
                return this.View();
            }

            return this.RedirectToAction("Settings", "User");
        }

        [HttpGet]
        public IActionResult Delete()
        {
            return this.View();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed()
        {
            Response response = await this.userService.DeleteUserAsync(this.currentUser.UserId!);

            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("UserDeleteErrors", response.Message!);
                return this.View();
            }

            return this.RedirectToAction("SignOut", "Auth");
        }
    }
}
