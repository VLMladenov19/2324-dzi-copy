using AutoMapper;
using AutoMapper.QueryableExtensions;
using Essentials.Results;
using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels.Product;
using HAR.Web.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace HAR.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly ICartService cartService;

        public ProductService(DataContext context, IMapper mapper, ICartService cartService)
        {
            this.context = context;
            this.mapper = mapper;
            this.cartService = cartService;
        }

        /// <summary>
        /// Finds a product by its id
        /// </summary>
        /// <param name="id">Products id</param>
        /// <returns>Product object</returns>
        public async Task<Product?> FindByIdAsync(Guid id)
        {
            return await this.context.Products
                .Where(a => a.Id == id)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// A pagination method for products
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<ProductViewModel>> FetchProductsAsync(IProductFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var productQuery = this.context.Products.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    productQuery = productQuery.Where(x => EF.Functions.Like(x.Name, searchPattern));
                }

                var totalCount = await productQuery.CountAsync();

                List<ProductViewModel> productItems;
                if (!string.IsNullOrEmpty(filterQuery.Category))
                {
                    var searchPattern = $"%{filterQuery.Category}%";
                    productQuery = productQuery.Where(x => EF.Functions.Like(x.Name, searchPattern));
                }

                productItems = await productQuery
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .ProjectTo<ProductViewModel>(this.mapper.ConfigurationProvider)
                    .ToListAsync();

                return PaginatedItemsResult<ProductViewModel>.ResultFrom(
                    productItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<ProductViewModel>.ResultFrom(
                    Array.Empty<ProductViewModel>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// Adds a product to database
        /// </summary>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> CreateProductAsync(Product product)
        {
            try
            {
                this.context.Products.Add(product);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктът успешно добавен."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктът не успя да се добави.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Removes a product from database
        /// </summary>
        /// <param name="product">Product object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> DeleteProductAsync(Product product)
        {
            try
            {
                this.context.Products.Remove(product);

                await this.context.SaveChangesAsync();

                await this.cartService.Update();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Продуктът бе успешно премахнат."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Продуктът не успя да се премахне.",
                    Details = ex.Message
                };
            }
        }
    }
}
