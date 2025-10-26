using System;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Application.Services;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HarborFlow.Backend.Tests.Services
{
    public class UserProfileServiceTests
    {
        private HarborFlowDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new HarborFlowDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private ILogger<UserProfileService> GetLogger()
        {
            return new Mock<ILogger<UserProfileService>>().Object;
        }

        private async Task<User> CreateAndAddUser(DbContext context, string username, string password)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = $"{username}@test.com",
                FullName = "Test User"
            };
            await context.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task GetUserProfileAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(GetUserProfileAsync_WithValidId_ShouldReturnUser));
            var user = await CreateAndAddUser(dbContext, "testuser", "pass123");
            var service = new UserProfileService(dbContext, GetLogger());

            // Act
            var result = await service.GetUserProfileAsync(user.UserId);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task UpdateUserProfileAsync_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(UpdateUserProfileAsync_WithValidData_ShouldUpdateUser));
            var user = await CreateAndAddUser(dbContext, "testuser", "pass123");
            var service = new UserProfileService(dbContext, GetLogger());

            var updatedUser = new User
            {
                UserId = user.UserId,
                FullName = "Updated Name",
                Email = "updated@test.com"
            };

            // Act
            var result = await service.UpdateUserProfileAsync(updatedUser);

            // Assert
            result.Should().BeTrue();
            var userInDb = await dbContext.Users.FindAsync(user.UserId);
            userInDb.Should().NotBeNull();
            userInDb!.FullName.Should().Be("Updated Name");
            userInDb.Email.Should().Be("updated@test.com");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithValidOldPassword_ShouldUpdatePasswordHash()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(ChangePasswordAsync_WithValidOldPassword_ShouldUpdatePasswordHash));
            var user = await CreateAndAddUser(dbContext, "testuser", "oldpass");
            var service = new UserProfileService(dbContext, GetLogger());
            var oldHash = user.PasswordHash;

            // Act
            var result = await service.ChangePasswordAsync(user.UserId, "oldpass", "newpass");

            // Assert
            result.Should().BeTrue();
            var userInDb = await dbContext.Users.FindAsync(user.UserId);
            userInDb.Should().NotBeNull();
            userInDb!.PasswordHash.Should().NotBe(oldHash);
            BCrypt.Net.BCrypt.Verify("newpass", userInDb.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task ChangePasswordAsync_WithInvalidOldPassword_ShouldReturnFalse()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(ChangePasswordAsync_WithInvalidOldPassword_ShouldReturnFalse));
            var user = await CreateAndAddUser(dbContext, "testuser", "oldpass");
            var service = new UserProfileService(dbContext, GetLogger());
            var oldHash = user.PasswordHash;

            // Act
            var result = await service.ChangePasswordAsync(user.UserId, "wrongpass", "newpass");

            // Assert
            result.Should().BeFalse();
            var userInDb = await dbContext.Users.FindAsync(user.UserId);
            userInDb.Should().NotBeNull();
            userInDb!.PasswordHash.Should().Be(oldHash);
        }
    }
}
