using AutoMapper;
using HAR.Data.Data;
using HAR.Data.Models;
using HAR.Service;
using HAR.Service.Implementations;
using Microsoft.EntityFrameworkCore;

namespace HAR.Tests
{
    public class AddressServiceTests
    {
        private readonly IMapper mapper;
        private readonly string userId = Guid.NewGuid().ToString(); // Initialize user ID here

        public AddressServiceTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task AddAddressAsync_ValidAddress_ReturnsSuccessfulResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "AddAddressAsync_ValidAddress_ReturnsSuccessfulResponse")
                .Options;

            using (var context = new DataContext(options))
            {
                var service = new AddressService(context, this.mapper, null); // No need for ICurrentUser service

                var address = new Address
                {
                    City = "Test City",
                    PostalCode = "12345",
                    Neighborhood = "Test Neighborhood",
                    Street = "Test Street",
                    StreetNumber = "123",
                    UserId = this.userId // Use initialized user ID
                };

                // Act
                var response = await service.AddAddressAsync(address);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Адресът е запазен успешно.", response.Message);
            }
        }

        [Fact]
        public async Task UpdateAddressAsync_ValidAddress_ReturnsSuccessfulResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UpdateAddressAsync_ValidAddress_ReturnsSuccessfulResponse")
                .Options;

            using (var context = new DataContext(options))
            {
                var service = new AddressService(context, this.mapper, null); // No need for ICurrentUser service

                var address = new Address
                {
                    City = "Test City",
                    PostalCode = "12345",
                    Neighborhood = "Test Neighborhood",
                    Street = "Test Street",
                    StreetNumber = "123",
                    UserId = this.userId // Use initialized user ID
                };

                context.Addresses.Add(address);
                await context.SaveChangesAsync();

                // Act
                var response = await service.UpdateAddressAsync(address);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Адресът е обновен успешно.", response.Message);
            }
        }

        [Fact]
        public async Task DeleteAddressAsync_ValidAddress_ReturnsSuccessfulResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "DeleteAddressAsync_ValidAddress_ReturnsSuccessfulResponse")
                .Options;

            using (var context = new DataContext(options))
            {
                var service = new AddressService(context, this.mapper, null); // No need for ICurrentUser service

                var address = new Address
                {
                    City = "Test City",
                    PostalCode = "12345",
                    Neighborhood = "Test Neighborhood",
                    Street = "Test Street",
                    StreetNumber = "123",
                    UserId = this.userId // Use initialized user ID
                };

                context.Addresses.Add(address);
                await context.SaveChangesAsync();

                // Act
                var response = await service.DeleteAddressAsync(address);

                // Assert
                Assert.True(response.IsSuccessful);
                Assert.Equal("Адръсът е изтрит успешно.", response.Message);
            }
        }
    }
}