using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels.Product;
using HAR.Web.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize(Roles = UserRole.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly IProductImageService productImageService;

        public ProductController(IProductService productService, ICategoryService categoryService, IProductImageService productImageService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.productImageService = productImageService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(ProductFilterQuery filterQuery)
        {
            var model = await this.productService.FetchProductsAsync(filterQuery);
            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (Guid.Empty == id)
            {
                return this.NotFound();
            }

            var product = await this.productService.FindByIdAsync(id);

            if (product == null)
            {
                return this.NotFound();
            }

            var model = new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Desc = product.Desc,
                Price = product.Price,
                CategoryName = product.CategoryName,
                Images = product.Images.ToList(),
                Reviews = product.Reviews.ToList()
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Admin(ProductFilterQuery filterQuery)
        {
            var model = await this.productService.FetchProductsAsync(filterQuery);
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateProductViewModel()
            {
                Categories = await this.categoryService.GetAll()
            };
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var product = new Product()
            {
                Name = model.Name,
                Desc = model.Desc,
                Price = model.Price,
                CategoryName = model.CategoryName
            };

            Response response = await this.productService.CreateProductAsync(product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("CreateProductErrors", response.Message!);
                return this.View(model);
            }

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var file in model.Images)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();

                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string base64 = Convert.ToBase64String(fileBytes);

                        var imageAddResponse = await this.productImageService.AddImage(new ProductImage()
                        {
                            Base64Content = base64,
                            FileName = file.FileName,
                            ProductId = product.Id,
                            Product = product
                        });

                        if (!imageAddResponse.IsSuccessful)
                        {
                            this.ModelState.AddModelError("CreateProductErrors", response.Message!);
                            return this.View(model);
                        }
                    }
                }
            }

            return this.RedirectToAction("Admin", "Product");
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            if (Guid.Empty == id)
            {
                return this.NotFound();
            }

            return this.View();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await this.productService.FindByIdAsync(id);

            if (product == null)
            {
                this.ModelState.AddModelError("ProductDeleteErrors", "Невалиден продукт.");
                return this.View(id);
            }

            Response response = await this.productService.DeleteProductAsync(product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("ProductDeleteErrors", response.Message!);
                return this.View(id);
            }

            return this.RedirectToAction("Admin", "Product");
        }
    }
}
