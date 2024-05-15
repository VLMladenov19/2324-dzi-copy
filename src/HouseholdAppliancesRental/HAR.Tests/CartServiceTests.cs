using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Contracts;
using HAR.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HAR.Tests
{
    public class CartServiceTests
    {
        private readonly DbContextOptions<DataContext> options;

        public CartServiceTests()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [Fact]
        public async Task FindByCurrentUserAsync_UserExists_ReturnsCart()
        {
            // Arrange
            var userId = "user2";
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe" };
            var cart = new Cart { UserId = userId };

            using (var context = new DataContext(this.options))
            {
                await context.Users.AddAsync(user);
                await context.Carts.AddAsync(cart);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(this.options))
            {
                var currentUserMock = new Mock<ICurrentUser>();
                currentUserMock.Setup(m => m.UserId).Returns(userId);
                var cartService = new CartService(context, currentUserMock.Object);

                // Act
                var result = await cartService.FindByCurrentUserAsync();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.UserId);
            }
        }

        [Fact]
        public async Task FindByCurrentUserAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = "nonexistentUser";

            using (var context = new DataContext(this.options))
            {
                var currentUserMock = new Mock<ICurrentUser>();
                currentUserMock.Setup(m => m.UserId).Returns(userId);
                var cartService = new CartService(context, currentUserMock.Object);

                // Act
                var result = await cartService.FindByCurrentUserAsync();

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddProductAsync_ProductAlreadyInCart_IncreasesProductQuantity()
        {
            // Arrange
            var userId = "user123";
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe" };
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Price = 10 };
            var cart = new Cart { UserId = userId };
            var cartProduct = new CartProduct { Cart = cart, Product = product, ProductQuantity = 1 };

            using (var context = new DataContext(this.options))
            {
                await context.Users.AddAsync(user);
                await context.Products.AddAsync(product);
                await context.Carts.AddAsync(cart);
                await context.CartProducts.AddAsync(cartProduct);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(this.options))
            {
                var currentUserMock = new Mock<ICurrentUser>();
                currentUserMock.Setup(m => m.UserId).Returns(userId);
                var cartService = new CartService(context, currentUserMock.Object);

                // Act
                var response = await cartService.AddProductAsync(cart, product);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Увелечена бройката на продукта в количката.", response.Message);

                var updatedCartProduct = await context.CartProducts.FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);
                Assert.NotNull(updatedCartProduct);
                Assert.Equal(2, updatedCartProduct.ProductQuantity);
            }
        }

        [Fact]
        public async Task Update_CartDoesNotExist_ReturnsErrorResponse()
        {
            // Arrange
            var userId = "nonexistentUser";

            using (var context = new DataContext(this.options))
            {
                var currentUserMock = new Mock<ICurrentUser>();
                currentUserMock.Setup(m => m.UserId).Returns(userId);
                var cartService = new CartService(context, currentUserMock.Object);

                // Act
                var response = await cartService.Update();

                // Assert
                Assert.False(response.IsSuccessful);
                Assert.Equal("Количката не е намерена.", response.Message);
            }
        }

        [Fact]
        public async Task ReduceProductAsync_ProductQuantityIsOne_RemovesProductFromCart()
        {
            // Arrange
            var userId = "user1";
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe" };
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product", Price = 10 };
            var cart = new Cart { UserId = userId };
            var cartProduct = new CartProduct { Cart = cart, Product = product, ProductQuantity = 1 };

            using (var context = new DataContext(this.options))
            {
                await context.Users.AddAsync(user);
                await context.Products.AddAsync(product);
                await context.Carts.AddAsync(cart);
                await context.CartProducts.AddAsync(cartProduct);
                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(this.options))
            {
                var currentUserMock = new Mock<ICurrentUser>();
                currentUserMock.Setup(m => m.UserId).Returns(userId);
                var cartService = new CartService(context, currentUserMock.Object);

                // Act
                var response = await cartService.ReduceProductAsync(cart, product);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Продуктът бе успешно премахнат от количката.", response.Message);

                var updatedCartProduct = await context.CartProducts.FirstOrDefaultAsync(cp => cp.Cart == cart && cp.Product == product);
                Assert.Null(updatedCartProduct);
            }
        }
    }
}
