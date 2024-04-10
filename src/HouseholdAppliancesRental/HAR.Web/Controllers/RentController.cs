using HAR.Common;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Web.Models.Rent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace HAR.Web.Controllers
{
    [Authorize]
    public class RentController : Controller
    {
        private readonly IUserService userService;
        private readonly ICurrentUser currentUser;
        private readonly ICartService cartService;
        private readonly IRentService rentService;
        private readonly IAddressService addressService;

        public RentController(IUserService userService, ICurrentUser currentUser, ICartService cartService, IRentService rentService, IAddressService addressService)
        {
            this.userService = userService;
            this.currentUser = currentUser;
            this.cartService = cartService;
            this.rentService = rentService;
            this.addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await this.userService.FindByIdAsync(this.currentUser.UserId!);
            Cart? cart = await this.cartService.FindByCurrentUserAsync();

            if (user == null || cart == null)
            {
                this.ModelState.AddModelError("ReviewRentErrors", "Потебителя или количката са невалидни.");
                return this.View();
            }

            decimal totalPrice = 0;

            if (cart.CartProducts.Count > 0)
            {
                totalPrice = cart.CartProducts
                    .Sum(cp => cp.ProductQuantity * cp.RentMonths * cp.Product.Price);
            }

            ReviewRentViewModel model = new ReviewRentViewModel()
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Addresses = user.Addresses.ToList(),
                CartProducts = cart.CartProducts.ToList(),
                TotalPrice = totalPrice
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReviewRentViewModel model)
        {
            Cart? cart = await this.cartService.FindByCurrentUserAsync();
            model.CartProducts = cart!.CartProducts.ToList();

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            User? user = await this.userService.FindByIdAsync(this.currentUser.UserId!);
            Address? address = await this.addressService.FindByIdAsync(model.AddressId);

            if (user == null || cart == null || address == null)
            {
                this.ModelState.AddModelError("ReviewRentErrors", "Потебител, количка или адрес са невалидни.");
                return this.View(model);
            }

            Rent rent = new Rent()
            {
                TotalPrice = model.TotalPrice,
                AddressId = address.Id,
                Address = address,
                RentalDate = DateTime.Now,
                ReturnDate = DateTime.Now,
                UserFullName = model.FullName,
                UserEmail = user.Email!,
                UserId = user.Id,
                User = user
            };

            List<RentProduct> rentProducts = new List<RentProduct>();
            foreach (var cartProduct in cart.CartProducts)
            {
                rentProducts.Add(new RentProduct()
                {
                    RentId = rent.Id,
                    Rent = rent,
                    ProductId = cartProduct.ProductId,
                    Product = cartProduct.Product,
                    ProductQuantity = cartProduct.ProductQuantity,
                    RentMonths = cartProduct.RentMonths
                });
            }
            rent.RentProducts = rentProducts;

            var response = await this.rentService.RentProducts(rent);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("ReviewRentErrors", response.Message!);
                return this.RedirectToAction("Index", "Rent");
            }

            try
            {
                string domain = "https://localhost:7073/";

                var options = new SessionCreateOptions()
                {
                    SuccessUrl = domain + $"Rent/Finish/{rent.Id}",
                    CancelUrl = domain + $"Rent/Finish/{rent.Id}",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    CustomerEmail = user.Email
                };

                foreach (var rentProduct in rent.RentProducts)
                {
                    var sessionListItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(rentProduct.Product.Price * 100),
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = rentProduct.Product.Name
                            }
                        },
                        Quantity = rentProduct.ProductQuantity
                    };
                    options.LineItems.Add(sessionListItem);
                }

                var service = new SessionService();

                Session session = service.Create(options);

                Response.Headers.Append("Location", session.Url);

                TempData["Session"] = session.Id;

                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("ReviewRentErrors", ex.Message);
                return this.View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Finish(Guid id)
        {
            if (Guid.Empty == id)
            {
                return this.NotFound();
            }

            SessionService service = new SessionService();
            Session? session = service.Get(TempData["Session"]!.ToString());

            Rent? rent = await this.rentService.FindByIdAsync(id);
            Cart? cart = await this.cartService.FindByCurrentUserAsync();

            if (rent == null || cart == null)
            {
                this.ModelState.AddModelError("ReviewRentErrors", "Наемът или количката са невалидни.");
                return this.RedirectToAction("Index", "Rent");
            }

            if (session.PaymentStatus.ToLower() != "paid")
            {
                Response response = await this.rentService.DeleteRent(rent);
                if (!response.IsSuccessful)
                {
                    this.ModelState.AddModelError("ReviewRentErrors", response.Message!);
                    return this.RedirectToAction("Index", "Rent");
                }
                return this.RedirectToAction("Index", "Rent");
            }

            foreach (var cp in cart.CartProducts)
            {
                await this.cartService.RemoveProductAsync(cart, cp.Product);
            }
            await this.cartService.Update();

            return this.View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Rent? rent = await this.rentService.FindByIdAsync(id);

            if (rent == null)
            {
                this.ModelState.AddModelError("RentDetailsErrors", "Rent not found.");
                return this.View(new RentViewModel());
            }

            decimal totalPrice = 0;

            if (rent.RentProducts.Count > 0)
            {
                totalPrice = rent.RentProducts
                    .Sum(cp => cp.ProductQuantity * cp.RentMonths * cp.Product.Price);
            }

            RentViewModel model = new RentViewModel()
            {
                RentProducts = rent.RentProducts.ToList(),
                TotalPrice = totalPrice,
            };

            return this.View(model);
        }
    }
}
