using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Service.Contracts;
using HAR.Web.Models;
using HAR.Web.Models.Rent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize(Roles = UserRole.Admin)]
    public class AdminController : Controller
    {
        private readonly IUserService userService;
        private readonly ICurrentUser currentUser;
        private readonly IRentService rentService;

        public AdminController(IUserService userService, ICurrentUser currentUser, IRentService rentService)
        {
            this.userService = userService;
            this.currentUser = currentUser;
            this.rentService = rentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Rents(RentFilterQuery filterQuery)
        {
            var rents = await this.rentService.FetchAllRentsAsync(filterQuery);
            return this.View(rents);
        }

        [HttpGet]
        public async Task<IActionResult> Users(GeneralFilterQuery filterQuery)
        {
            var model = await this.userService.FetchUsersAsync(filterQuery);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeRole(string id)
        {
            if (id == this.currentUser.UserId && User.IsInRole(UserRole.Admin))
            {
                this.TempData["ChangeUserRoleErrors"] = "Не може да промениш ролята на текущия администратор.";
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.userService.ChangeRoleAsync(id);

            if (!response.IsSuccessful)
            {
                this.TempData["ChangeUserRoleErrors"] = response.Message!;
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public IActionResult DeleteUser(string id)
        {
            if (id == string.Empty)
            {
                return this.NotFound();
            }

            if (id == this.currentUser.UserId && User.IsInRole(UserRole.Admin))
            {
                this.TempData["UserDeleteErrors"] = "Не може да изтриеш текущия администратор.";
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.View();
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            Response response = await this.userService.DeleteUserAsync(id);

            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("UserDeleteErrors", response.Message!);
                return this.View();
            }

            return this.RedirectToAction("Users", "Admin");
        }
    }
}
