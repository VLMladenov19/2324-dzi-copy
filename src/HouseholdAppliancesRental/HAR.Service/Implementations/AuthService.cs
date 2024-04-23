using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HAR.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ICartService cartService;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ICartService cartService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.cartService = cartService;
        }

        /// <summary>
        /// Checks if user is in database
        /// </summary>
        /// <param name="email">Users email</param>
        /// <returns>Boolean</returns>
        public async Task<bool> UserExistsAsync(string email)
        {
            return await this.userManager.FindByEmailAsync(email) != null;
        }

        /// <summary>
        /// Creates the needed cookies and claims for the user
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="model"></param>
        /// <returns>Custom response object</returns>
        public async Task<Response> AuthenticateUserAsync(HttpContext httpContext, SignInViewModel model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new Response
                {
                    IsSuccessful = false,
                    Message = "Инвалидни идентификационни данни."
                };
            }

            var isPasswordCorrect = await this.userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordCorrect)
            {
                return new Response
                {
                    IsSuccessful = false,
                    Message = "Инвалидни идентификационни данни."
                };
            }

            var claims = await this.userManager.GetClaimsAsync(user);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email!));

            var userRoles = await this.userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties { IsPersistent = true });

            return new Response
            {
                IsSuccessful = true,
                Message = "Потребителя се вписа успешно."
            };
        }

        /// <summary>
        /// Add a user to database
        /// </summary>
        /// <param name="model">A model of a user filled with need values</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> CreateUserAsync(SignUpViewModel model)
        {
            User user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var response = await this.userManager.CreateAsync(user, model.Password);
            if (!response.Succeeded)
            {
                return new Response
                {
                    IsSuccessful = false,
                    Message = response.Errors.FirstOrDefault()!.Description
                };
            }

            if (!await this.roleManager.RoleExistsAsync(model.Role))
            {
                await this.roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            if (await this.roleManager.RoleExistsAsync(model.Role))
            {
                await this.userManager.AddToRoleAsync(user, model.Role);
            }

            Response cartResponse = await this.cartService.CreateCartAsync(user);

            if (!cartResponse.IsSuccessful)
            {
                await this.userManager.DeleteAsync(user);

                return new Response
                {
                    IsSuccessful = false,
                    Message = cartResponse.Message
                };
            }

            return new Response
            {
                IsSuccessful = true,
                Message = "Създаването на потребителя е успешно."
            };
        }

        /// <summary>
        /// Seeds an admin profile to the database if there isn't any
        /// </summary>
        /// <returns></returns>
        public async Task SeedAdminUserAsync()
        {
            var adminUser = await this.userManager.FindByEmailAsync("Admin@gmail.com");

            if (adminUser == null)
            {
                SignUpViewModel model = new SignUpViewModel()
                {
                    Email = "Admin@gmail.com",
                    Password = "Pass@123",
                    ConfirmPassword = "Pass@123",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Role = UserRole.Admin
                };

                await this.CreateUserAsync(model);
            }
        }
    }
}
