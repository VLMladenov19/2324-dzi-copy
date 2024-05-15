using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HAR.Tests
{
    public class ReviewServiceTests
    {
        [Fact]
        public async Task CreateReviewAsync_Success_ReturnsSuccessfulResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateReviewAsync_Success_ReturnsSuccessfulResponse")
                .Options;

            using (var context = new DataContext(options))
            {
                var reviewService = new ReviewService(context);

                var review = new Review
                {
                    Rating = 5, // Example rating value
                    Comment = "Test comment", // Example comment value
                    UserId = "user123", // Example user ID
                    ProductId = Guid.NewGuid() // Example product ID
                };

                // Act
                var response = await reviewService.CreateReviewAsync(review);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Отзив създаден успешно.", response.Message);
            }
        }

        [Fact]
        public async Task CreateReviewAsync_Failure_ReturnsErrorResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CreateReviewAsync_Failure_ReturnsErrorResponse")
                .Options;

            using (var context = new DataContext(options))
            {
                // Mock setup to throw an exception when adding a review
                var mockDbSet = new Mock<DbSet<Review>>();
                mockDbSet.Setup(m => m.Add(It.IsAny<Review>())).Throws(new Exception("Simulated error"));

                context.Database.EnsureDeleted(); // Ensure database is empty
                context.Database.EnsureCreated(); // Ensure database is created

                // Use the mock DbSet in the context
                context.Reviews = mockDbSet.Object;

                var reviewService = new ReviewService(context);

                var review = new Review
                {
                    Rating = 5, // Example rating value
                    Comment = "Test comment", // Example comment value
                    UserId = "user123", // Example user ID
                    ProductId = Guid.NewGuid() // Example product ID
                };

                // Act
                var response = await reviewService.CreateReviewAsync(review);

                // Assert
                Assert.False(response.IsSuccessful);
                Assert.Equal("Отзивът не успя да се добави.", response.Message);
                Assert.Contains("Simulated error", response.Details);
            }
        }
    }
}
