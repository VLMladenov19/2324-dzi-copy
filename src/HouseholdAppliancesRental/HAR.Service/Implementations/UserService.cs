using Essentials.Results;
using HAR.Common;
using HAR.Common.Models.Auth;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HAR.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly ICurrentUser currentUser;

        public UserService(DataContext context, UserManager<User> userManager, ICurrentUser currentUser)
        {
            this.context = context;
            this.userManager = userManager;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Finds a user by his id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User object</returns>
        public async Task<User?> FindByIdAsync(string userId)
        {
            return await this.context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// A pagination method for users
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<UserViewModel>> FetchUsersAsync(IFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var userQuery = this.context.Users
                    .Join(
                        context.UserRoles,
                        u => u.Id,
                        ur => ur.UserId,
                        (u, ur) => new
                        {
                            User = u,
                            UserRole = ur
                        })
                    .Join(
                        context.Roles,
                        ur => ur.UserRole.RoleId,
                        r => r.Id,
                        (ur, r) => new
                        {
                            ur.User,
                            Role = r.Name
                        })
                    .Select(u => new UserViewModel
                    {
                        Id = u.User.Id,
                        FirstName = u.User.FirstName,
                        LastName = u.User.LastName,
                        Email = u.User.Email!,
                        Role = u.Role!
                    })
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    userQuery = userQuery.Where(x => EF.Functions.Like(x.Email, searchPattern));
                }

                var totalCount = await userQuery.CountAsync();

                var userItems = await userQuery
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .OrderByDescending(u => u.Id == this.currentUser.UserId)
                        .ThenBy(u => u.Id)
                    .ToListAsync();

                return PaginatedItemsResult<UserViewModel>.ResultFrom(
                    userItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<UserViewModel>.ResultFrom(
                    Array.Empty<UserViewModel>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// Changes the role of a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Custrom response object</returns>
        public async Task<Response> ChangeRoleAsync(string userId)
        {
            User? user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Потребител не е намерен."
                };
            }

            var isAdmin = await userManager.IsInRoleAsync(user, UserRole.Admin);

            if (isAdmin)
            {
                await userManager.AddToRoleAsync(user, UserRole.User);
                await userManager.RemoveFromRoleAsync(user, UserRole.Admin);
            }
            else
            {
                await userManager.AddToRoleAsync(user, UserRole.Admin);
                await userManager.RemoveFromRoleAsync(user, UserRole.User);
            }

            return new Response()
            {
                IsSuccessful = true,
                Message = "Потребителската роля бе сменена."
            };
        }

        /// <summary>
        /// Deletes a user from database
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> DeleteUserAsync(string userId)
        {
            User? user = await this.userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Потребителят не бе намерен."
                };
            }

            var isAdmin = await userManager.IsInRoleAsync(user, UserRole.Admin);

            if (isAdmin)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Не може да изтриеш администратор."
                };
            }

            var response = await this.userManager.DeleteAsync(user);

            if (!response.Succeeded)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Потребителят не бе изтрит успешно."
                };
            }

            return new Response()
            {
                IsSuccessful = true,
                Message = "Потребителят бе изтрит успешно."
            };
        }
    }
}
