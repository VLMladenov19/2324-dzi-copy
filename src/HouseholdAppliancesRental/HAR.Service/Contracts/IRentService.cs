using Essentials.Results;
using HAR.Common;
using HAR.Data.Models;
using HAR.Service.ViewModels;

namespace HAR.Service.Contracts
{
    public interface IRentService
    {
        Task<Rent?> FindByIdAsync(Guid rentId);
        Task<PaginatedItemsResult<Rent>> FetchAllRentsAsync(IFilterQuery filterQuery);
        Task<PaginatedItemsResult<Rent>> FetchCurrentUserRentsAsync(IFilterQuery filterQuery);
        Task<Response> RentProducts(Rent rent);
        Task<Response> DeleteRent(Rent rent);
    }
}
