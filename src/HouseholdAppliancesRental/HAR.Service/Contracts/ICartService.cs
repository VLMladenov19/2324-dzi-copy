using HAR.Common;
using HAR.Data.Models;

namespace HAR.Service.Contracts
{
    public interface ICartService
    {
        Task<Response> Update();
        Task<Cart?> FindByCurrentUserAsync();
        Task<Response> CreateCartAsync(User user);
        Task<Response> AddProductAsync(Cart cart, Product product);
        Task<Response> ReduceProductAsync(Cart cart, Product product);
        Task<Response> RemoveProductAsync(Cart cart, Product product);
        Task<Response> AddMonthAsync(Cart cart, Product product);
        Task<Response> ReduceMonthAsync(Cart cart, Product product);
    }
}
