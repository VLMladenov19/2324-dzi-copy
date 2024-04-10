using HAR.Common;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels.Address;
using HAR.Web.Models;
using HAR.Web.Models.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HAR.Web.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService addressService;
        private readonly ICurrentUser currentUser;

        public AddressController(IAddressService addressService, ICurrentUser currentUser)
        {
            this.addressService = addressService;
            this.currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Index(GeneralFilterQuery filterQuery)
        {
            var model = await this.addressService.FetchAddressesAsync(filterQuery);
            return this.View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new AddAddressViewModel();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddAddressViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            Address address = new Address()
            {
                City = model.City,
                PostalCode = model.PostalCode,
                Neighborhood = model.Neighborhood,
                Street = model.Street,
                StreetNumber = model.StreetNumber,
                BlockNumber = model.BlockNumber,
                Entrance = model.Entrance,
                Floor = model.Floor,
                Apartment = model.Apartment,
                UserId = this.currentUser.UserId!
            };

            Response response = await this.addressService.AddAddressAsync(address);

            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("AddAddressErrors", response.Message!);
                return this.View(model);
            }

            return this.RedirectToAction("Index", "Address");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.NotFound();
            }

            var address = await this.addressService.FindByIdAsync(id);

            if (address == null)
            {
                return this.NotFound();
            }

            var model = new AddressViewModel()
            {
                City = address.City,
                PostalCode = address.PostalCode,
                Neighborhood = address.Neighborhood,
                Street = address.Street,
                StreetNumber = address.StreetNumber,
                BlockNumber = address.BlockNumber,
                Entrance = address.Entrance,
                Floor = address.Floor,
                Apartment = address.Apartment
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddressViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var address = new Address()
            {
                Id = model.Id,
                City = model.City,
                PostalCode = model.PostalCode,
                Neighborhood = model.Neighborhood,
                Street = model.Street,
                StreetNumber = model.StreetNumber,
                BlockNumber = model.BlockNumber,
                Entrance = model.Entrance,
                Floor = model.Floor,
                Apartment = model.Apartment,
                UserId = this.currentUser.UserId!
            };

            Response response = await this.addressService.UpdateAddressAsync(address);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("AddressDetailsErrors", response.Message!);
                return this.View(address);
            }

            return this.RedirectToAction("Index", "Address");
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return this.NotFound();
            }

            return this.View();
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var address = await this.addressService.FindByIdAsync(id);

            if (address == null)
            {
                this.ModelState.AddModelError("AddressDeleteErrors", "Невалиден адрес.");
                return this.View(id);
            }

            Response response = await this.addressService.DeleteAddressAsync(address);
            if (!response.IsSuccessful)
            {
                this.ModelState.AddModelError("AddressDeleteErrors", response.Message!);
                return this.View(id);
            }

            return this.RedirectToAction("Index", "Address");
        }
    }
}
