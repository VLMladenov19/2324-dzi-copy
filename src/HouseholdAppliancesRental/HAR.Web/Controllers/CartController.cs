using HAR.Common;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Web.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly IProductService productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            this.cartService = cartService;
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();

            if (cart == null)
            {
                this.ModelState.AddModelError("CartViewErrors", "Количката не бе намерена.");
                return this.View(new CartViewModel());
            }

            int totalMonths = 0;
            decimal totalPrice = 0;

            if (cart.CartProducts.Count > 0)
            {
                totalMonths = cart.CartProducts.Max(cp => cp.RentMonths);
                totalPrice = cart.CartProducts
                    .Sum(cp => cp.ProductQuantity * cp.RentMonths * cp.Product.Price);
            }

            CartViewModel model = new CartViewModel()
            {
                MonthPrice = cart.MonthPrice,
                CartProducts = cart.CartProducts.ToList(),
                TotalMonths = totalMonths,
                TotalPrice = totalPrice,
            };

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct(Guid productId)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            Product? product = await this.productService.FindByIdAsync(productId);

            if (cart == null || product == null)
            {
                this.ModelState.AddModelError("AddToCartErrors", "Количката или продуктът не бяха намерени.");
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.cartService.AddProductAsync(cart, product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("AddToCartErrors", response.Message!);
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> ReduceProduct(Guid productId)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            Product? product = await this.productService.FindByIdAsync(productId);

            if (cart == null || product == null)
            {
                this.ModelState.AddModelError("ReduceFromCartErrors", "Количката или продуктът не бяха намерени.");
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.cartService.ReduceProductAsync(cart, product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("ReduceFromCartErrors", response.Message!);
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> RemoveProduct(Guid productId)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            Product? product = await this.productService.FindByIdAsync(productId);

            if (cart == null || product == null)
            {
                this.ModelState.AddModelError("RemoveFromCartErrors", "Количката или продуктът не бяха намерени..");
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.cartService.RemoveProductAsync(cart, product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("RemoveFromCartErrors", response.Message!);
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> AddMonth(Guid productId)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            Product? product = await this.productService.FindByIdAsync(productId);

            if (cart == null || product == null)
            {
                this.ModelState.AddModelError("MonthErrors", "Количката или продуктът не бяха намерени.");
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.cartService.AddMonthAsync(cart, product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("MonthErrors", response.Message!);
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }

        [HttpGet]
        public async Task<IActionResult> ReduceMonth(Guid productId)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            Product? product = await this.productService.FindByIdAsync(productId);

            if (cart == null || product == null)
            {
                this.ModelState.AddModelError("MonthErrors", "Количката или продуктът не бяха намерени.");
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            Response response = await this.cartService.ReduceMonthAsync(cart, product);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("MonthErrors", response.Message!);
                return this.Redirect(Request.Headers.Referer.ToString());
            }

            return this.Redirect(Request.Headers.Referer.ToString());
        }
    }
}
