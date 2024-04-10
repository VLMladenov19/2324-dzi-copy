using HAR.Service.Contracts;
using HAR.Service.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HAR.Service
{
    /// <summary>
    /// Register various services and their corresponding implementations
    /// </summary>
    public static class DependencyInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IAddressService, AddressService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<ICartService, CartService>()
                .AddScoped<ICategoryService, CategoryService>()
                .AddScoped<ICurrentUser, CurrentUser>()
                .AddScoped<IProductImageService, ProductImageService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IRentService, RentService>()
                .AddScoped<IReviewService, ReviewService>()
                .AddScoped<IUserService, UserService>();
        }
    }
}
