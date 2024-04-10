using AutoMapper;
using AutoMapper.QueryableExtensions;
using Essentials.Results;
using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.Address;
using Microsoft.EntityFrameworkCore;

namespace HAR.Service.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly ICurrentUser currentUser;

        public AddressService(DataContext context, IMapper mapper, ICurrentUser currentUser)
        {
            this.context = context;
            this.mapper = mapper;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Finds an address based on his id
        /// </summary>
        /// <param name="id">The id of an address</param>
        /// <returns>An address including his user</returns>
        public async Task<Address?> FindByIdAsync(Guid id)
        {
            return await this.context.Addresses
                .Where(a => a.Id == id)
                .Include(a => a.User)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// A pagination method for addresses
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<AddressViewModel>> FetchAddressesAsync(IFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var addressQuery = this.context.Addresses
                    .Where(a => a.UserId == this.currentUser.UserId)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    addressQuery = addressQuery.Where(x => EF.Functions.Like(x.City, searchPattern));
                }

                var totalCount = await addressQuery.CountAsync();

                var addressItems = await addressQuery
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .ProjectTo<AddressViewModel>(this.mapper.ConfigurationProvider)
                    .ToListAsync();

                return PaginatedItemsResult<AddressViewModel>.ResultFrom(
                    addressItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<AddressViewModel>.ResultFrom(
                    Array.Empty<AddressViewModel>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// Adds an address to database
        /// </summary>
        /// <param name="address">Address object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> AddAddressAsync(Address address)
        {
            try
            {
                this.context.Addresses.Add(address);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Адресът е запазен успешно."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Адресът не успя да се запази.",
                    Details = ex.Message
                };
            }
        }


        /// <summary>
        /// Updates an address to database
        /// </summary>
        /// <param name="address">Address object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> UpdateAddressAsync(Address address)
        {
            try
            {
                this.context.Addresses.Update(address);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Адресът е обновен успешно."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Адресът не успя да се обнови.",
                    Details = ex.Message
                };
            }
        }


        /// <summary>
        /// Deletes an address to database
        /// </summary>
        /// <param name="address">Address object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> DeleteAddressAsync(Address address)
        {
            try
            {
                this.context.Addresses.Remove(address);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Адръсът е изтрит успешно."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Адресът не успя да се изтрие.",
                    Details = ex.Message
                };
            }
        }
    }
}
