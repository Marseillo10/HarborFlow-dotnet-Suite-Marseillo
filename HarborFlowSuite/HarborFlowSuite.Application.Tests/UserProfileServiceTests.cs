using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Application.Tests
{
    public class UserProfileServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUserService = new Moq.Mock<HarborFlowSuite.Infrastructure.Services.ICurrentUserService>();
            return new ApplicationDbContext(options, mockCurrentUserService.Object);
        }

        [Fact]
        public async Task GetUserProfileAsync_UserExists_ReturnsUserProfile()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var user = new User { Id = Guid.NewGuid(), FirebaseUid = "test-uid", FullName = "Test User", Email = "test@example.com" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var service = new UserProfileService(dbContext);

            // Act
            var result = await service.GetUserProfileAsync(user.FirebaseUid, user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.FullName, result.FullName);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UserExists_UpdatesUserProfile()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var user = new User { Id = Guid.NewGuid(), FirebaseUid = "test-uid", FullName = "Test User", Email = "test@example.com" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var service = new UserProfileService(dbContext);
            var updatedProfile = new UserProfileDto { FullName = "Updated Name", Email = "updated@example.com" };

            // Act
            await service.UpdateUserProfileAsync(user.FirebaseUid, updatedProfile);
            var updatedUser = await dbContext.Users.FindAsync(user.Id);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(updatedProfile.FullName, updatedUser.FullName);
            Assert.Equal(updatedProfile.Email, updatedUser.Email);
        }
    }
}
