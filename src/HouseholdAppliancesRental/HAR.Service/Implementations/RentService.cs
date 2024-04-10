using Essentials.Results;
using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HAR.Service.Implementations
{
    public class RentService : IRentService
    {
        private readonly DataContext context;
        private readonly ICurrentUser currentUser;

        public RentService(DataContext context, ICurrentUser currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Finds a rent by id
        /// </summary>
        /// <param name="rentId">Rents id</param>
        /// <returns>Rent object</returns>
        public async Task<Rent?> FindByIdAsync(Guid rentId)
        {
            return await this.context.Rents
                .Where(r => r.Id == rentId)
                    .Include(r => r.RentProducts)
                        .ThenInclude(rp => rp.Product)
                            .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// A pagination method for rents
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<Rent>> FetchAllRentsAsync(IFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var rentQuery = this.context.Rents
                    .Include(r => r.RentProducts)
                        .ThenInclude(rp => rp.Product)
                            .ThenInclude(p => p.Images)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    rentQuery = rentQuery.Where(x => EF.Functions.Like(x.UserEmail, searchPattern));
                }

                var totalCount = await rentQuery.CountAsync();

                var rentItems = await rentQuery
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .ToListAsync();

                return PaginatedItemsResult<Rent>.ResultFrom(
                    rentItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<Rent>.ResultFrom(
                    Array.Empty<Rent>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// A pagination method for current users rents
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<Rent>> FetchCurrentUserRentsAsync(IFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var rentQuery = this.context.Rents
                    .Where(r => r.UserId == this.currentUser.UserId)
                    .Include(r => r.RentProducts)
                        .ThenInclude(rp => rp.Product)
                            .ThenInclude(p => p.Images)
                    .AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    rentQuery = rentQuery.Where(x => EF.Functions.Like(x.UserEmail, searchPattern));
                }

                var totalCount = await rentQuery.CountAsync();

                var rentItems = await rentQuery
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .ToListAsync();

                return PaginatedItemsResult<Rent>.ResultFrom(
                    rentItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<Rent>.ResultFrom(
                    Array.Empty<Rent>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// Rents the products 
        /// </summary>
        /// <param name="rent">Rent object</param>
        /// <returns></returns>
        public async Task<Response> RentProducts(Rent rent)
        {
            try
            {
                this.context.Rents.Add(rent);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктите са успешно наети.",
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктите не успяха да се наемат.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Deletes a rent from database
        /// </summary>
        /// <param name="rent">Rent object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> DeleteRent(Rent rent)
        {
            try
            {
                this.context.Rents.Remove(rent);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Наем успешно изтрит.",
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Наемът не успя да се изтрие.",
                    Details = ex.Message
                };
            }
        }
    }
}
