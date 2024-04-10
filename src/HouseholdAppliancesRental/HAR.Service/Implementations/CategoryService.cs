using AutoMapper;
using AutoMapper.QueryableExtensions;
using Essentials.Results;
using HAR.Common;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.ViewModels;
using HAR.Service.ViewModels.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HAR.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public CategoryService(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all categories created so far
        /// </summary>
        /// <returns>List of Category objects</returns>
        public async Task<List<Category>> GetAll()
        {
            return await this.context.Categories.ToListAsync();
        }

        /// <summary>
        /// A pagination method for categories
        /// </summary>
        /// <param name="filterQuery">The filtering categories</param>
        /// <returns>An object with pages count, size and also the items</returns>
        public async Task<PaginatedItemsResult<CategoryViewModel>> FetchCategoriesAsync(IFilterQuery filterQuery)
        {
            try
            {
                filterQuery.Normalize();
                var categoryQuery = this.context.Categories.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filterQuery.SearchQuery))
                {
                    var searchPattern = $"%{filterQuery.SearchQuery}%";
                    categoryQuery = categoryQuery.Where(x => EF.Functions.Like(x.Name, searchPattern));
                }

                var totalCount = await categoryQuery.CountAsync();

                var categoryItems = await categoryQuery
                    .Skip((filterQuery.Page - 1) * filterQuery.PageSize)
                    .Take(filterQuery.PageSize)
                    .ProjectTo<CategoryViewModel>(this.mapper.ConfigurationProvider)
                    .ToListAsync();

                return PaginatedItemsResult<CategoryViewModel>.ResultFrom(
                    categoryItems,
                    filterQuery.PageSize,
                    filterQuery.Page,
                    totalCount);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return PaginatedItemsResult<CategoryViewModel>.ResultFrom(
                    Array.Empty<CategoryViewModel>(),
                    filterQuery.PageSize,
                    filterQuery.Page,
                    0);
            }
        }

        /// <summary>
        /// Finds a category by its name
        /// </summary>
        /// <param name="name">Category name</param>
        /// <returns>Return a category object</returns>
        public async Task<Category?> FindByNameAsync(string name)
        {
            return await this.context.Categories
                .Where(a => a.Name == name)
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Adds a categort to database
        /// </summary>
        /// <param name="category">Category object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> CreateCategoryAsync(Category category)
        {
            try
            {
                this.context.Categories.Add(category);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Категорията успешно създадена."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Категорията не успя да се създаде.",
                    Details = ex.Message
                };
            }
        }

        /// <summary>
        /// Removes a categort from database
        /// </summary>
        /// <param name="category">Category object</param>
        /// <returns>Custom response object</returns>
        public async Task<Response> DeleteCategoryAsync(Category category)
        {
            if (!category.ChildCategories.IsNullOrEmpty())
            {
                foreach (var child in category.ChildCategories)
                {
                    child.ParentCategoryName = null;
                    child.ParentCategory = null;
                }

                try
                {
                    await this.context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return new Response()
                    {
                        IsSuccessful = false,
                        Message = "Неуспешно премахнати отношения.",
                        Details = ex.Message
                    };
                }
            }

            try
            {
                this.context.Categories.Remove(category);

                await this.context.SaveChangesAsync();

                return new Response()
                {
                    IsSuccessful = true,
                    Message = "Категорията бе успешно изтрита."
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    IsSuccessful = false,
                    Message = "Категорията не успя да се изтрие.",
                    Details = ex.Message
                };
            }
        }
    }
}
