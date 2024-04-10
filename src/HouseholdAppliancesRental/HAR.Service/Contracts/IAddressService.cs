using Essentials.Results;
using HAR.Common;
using HAR.Data.Models;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.Address;

namespace HAR.Service.Contracts
{
    public interface IAddressService
    {
        Task<Address?> FindByIdAsync(Guid id);
        Task<PaginatedItemsResult<AddressViewModel>> FetchAddressesAsync(IFilterQuery filterQuery);
        Task<Response> AddAddressAsync(Address address);
        Task<Response> UpdateAddressAsync(Address address);
        Task<Response> DeleteAddressAsync(Address address);
    }
}
