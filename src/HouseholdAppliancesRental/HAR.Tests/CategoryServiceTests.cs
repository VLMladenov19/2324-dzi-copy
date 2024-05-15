using AutoMapper;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HAR.Tests
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetAll_ReturnsListOfCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Name = "Category 1" },
                new Category { Name = "Category 2" },
                new Category { Name = "Category 3" }
            };

            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "GetAll_ReturnsListOfCategories")
                .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(dbContextOptions))
            {
                var mapperMock = new Mock<IMapper>();
                var service = new CategoryService(context, mapperMock.Object);

                // Act
                var result = await service.GetAll();

                // Assert
                Assert.Equal(categories.Count, result.Count);
                Assert.Equal(categories.Select(c => c.Name), result.Select(c => c.Name));
            }
        }

        [Fact]
        public async Task FindByNameAsync_ExistingName_ReturnsCategory()
        {
            // Arrange
            var categoryName = "Category 1";
            var category = new Category { Name = categoryName };

            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "FindByNameAsync_ExistingName_ReturnsCategory")
                .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(dbContextOptions))
            {
                var mapperMock = new Mock<IMapper>();
                var service = new CategoryService(context, mapperMock.Object);

                // Act
                var result = await service.FindByNameAsync(categoryName);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(categoryName, result.Name);
            }
        }

        [Fact]
        public async Task FindByNameAsync_NonExistingName_ReturnsNull()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "FindByNameAsync_NonExistingName_ReturnsNull")
                .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                var mapperMock = new Mock<IMapper>();
                var service = new CategoryService(context, mapperMock.Object);

                // Act
                var result = await service.FindByNameAsync("Non-existing Category");

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidCategory_ReturnsSuccessResponse()
        {
            // Arrange
            var category = new Category { Name = "New Category" };

            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateCategoryAsync_ValidCategory_ReturnsSuccessResponse")
                .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                var mapperMock = new Mock<IMapper>();
                var service = new CategoryService(context, mapperMock.Object);

                // Act
                var response = await service.CreateCategoryAsync(category);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Категорията успешно създадена.", response.Message);

                // Check if the category has been added to the database
                var createdCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
                Assert.NotNull(createdCategory);
                Assert.Equal(category.Name, createdCategory.Name);
            }
        }

        [Fact]
        public async Task DeleteCategoryAsync_ValidCategory_ReturnsSuccessResponse()
        {
            // Arrange
            var category = new Category { Name = "Category to delete" };

            var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "DeleteCategoryAsync_ValidCategory_ReturnsSuccessResponse")
                .Options;

            using (var context = new DataContext(dbContextOptions))
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(dbContextOptions))
            {
                var mapperMock = new Mock<IMapper>();
                var service = new CategoryService(context, mapperMock.Object);

                // Act
                var response = await service.DeleteCategoryAsync(category);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Категорията бе успешно изтрита.", response.Message);

                // Check if the category has been removed from the database
                var deletedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);
                Assert.Null(deletedCategory);
            }
        }
    }
}
