using HAR.Common;
using HAR.Service.ViewModels.Auth;
using Microsoft.AspNetCore.Http;

namespace HAR.Service.Contracts
{
    public interface IAuthService
    {
        Task<bool> UserExistsAsync(string email);
        Task<Response> AuthenticateUserAsync(HttpContext httpContext, SignInViewModel model);
        Task<Response> CreateUserAsync(SignUpViewModel model);
        Task SeedAdminUserAsync();
    }
}
