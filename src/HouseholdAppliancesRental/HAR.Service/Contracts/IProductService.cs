using Essentials.Results;
using HAR.Common;
using HAR.Data.Models;
using HAR.Service.ViewModels.Product;
using HAR.Web.Models.Product;

namespace HAR.Service.Contracts
{
    public interface IProductService
    {
        Task<Product?> FindByIdAsync(Guid id);
        Task<PaginatedItemsResult<ProductViewModel>> FetchProductsAsync(IProductFilterQuery filterQuery);
        Task<Response> CreateProductAsync(Product product);
        Task<Response> DeleteProductAsync(Product product);
    }
}
