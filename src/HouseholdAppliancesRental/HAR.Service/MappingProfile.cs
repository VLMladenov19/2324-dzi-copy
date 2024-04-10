using AutoMapper;
using HAR.Data.Models;
using HAR.Service.ViewModels.Address;
using HAR.Service.ViewModels.Category;
using HAR.Service.ViewModels.Product;

namespace HAR.Service
{
    /// <summary>
    /// Configure mapping between sources and their destination types
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Address, AddressViewModel>();
            this.CreateMap<Category, CategoryViewModel>();
            this.CreateMap<Product, ProductViewModel>();
        }
    }
}
