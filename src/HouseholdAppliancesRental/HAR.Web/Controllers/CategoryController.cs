using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Web.Models;
using HAR.Web.Models.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize(Roles = UserRole.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(GeneralFilterQuery filterQuery)
        {
            var model = await this.categoryService.FetchCategoriesAsync(filterQuery);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateCategoryViewModel()
            {
                Categories = await this.categoryService.GetAll()
            };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var category = new Category()
            {
                Name = model.Name,
                ParentCategoryName = model.ParentCategoryName
            };

            Response response = await this.categoryService.CreateCategoryAsync(category);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("CreateCategoryErrors", response.Message!);
                return this.View(model);
            }

            return this.RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public IActionResult Delete(string name)
        {
            if (name == string.Empty)
            {
                return this.NotFound();
            }

            return this.View();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string name)
        {
            var category = await this.categoryService.FindByNameAsync(name);

            if (category == null)
            {
                this.ModelState.AddModelError("CategoryDeleteErrors", "Невалидна категория.");
                return this.View(name);
            }

            Response response = await this.categoryService.DeleteCategoryAsync(category);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("CategoryDeleteErrors", response.Message!);
                return this.View(name);
            }

            return this.RedirectToAction("Index", "Category");
        }
    }
}
