using Essentials.Results;
using HAR.Common;
using HAR.Data.Models;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.User;

namespace HAR.Service.Contracts
{
    public interface IUserService
    {
        Task<User?> FindByIdAsync(string userId);
        Task<PaginatedItemsResult<UserViewModel>> FetchUsersAsync(IFilterQuery filterQuery);
        Task<Response> ChangeRoleAsync(string userId);
        Task<Response> DeleteUserAsync(string userId);
    }
}
