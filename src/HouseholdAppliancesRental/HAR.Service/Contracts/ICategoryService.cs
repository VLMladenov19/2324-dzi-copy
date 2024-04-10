using Essentials.Results;
using HAR.Common;
using HAR.Data.Models;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.Category;

namespace HAR.Service.Contracts
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAll();
        Task<Category?> FindByNameAsync(string name);
        Task<PaginatedItemsResult<CategoryViewModel>> FetchCategoriesAsync(IFilterQuery filterQuery);
        Task<Response> CreateCategoryAsync(Category category);
        Task<Response> DeleteCategoryAsync(Category category);
    }
}
