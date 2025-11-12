using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Services;
using HarborFlowSuite.Server.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Server.Tests
{
    public class UserProfileControllerTests
    {
        private UserProfileController CreateController(IUserProfileService userProfileService, string userId, string email)
        {
            var controller = new UserProfileController(userProfileService);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, email)
                }, "mock"));

                controller.ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                };
            }
            return controller;
        }

        [Fact]
        public async Task GetUserProfile_UserIsAuthenticated_ReturnsUserProfile()
        {
            // Arrange
            var mockService = new Mock<IUserProfileService>();
            var userId = "test-uid";
            var userEmail = "test@example.com";
            var userProfile = new UserProfileDto { FullName = "Test User", Email = userEmail };
            mockService.Setup(s => s.GetUserProfileAsync(userId, userEmail)).ReturnsAsync(userProfile);
            var controller = CreateController(mockService.Object, userId, userEmail);

            // Act
            var result = await controller.GetUserProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProfile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal(userProfile.FullName, returnedProfile.FullName);
        }

        [Fact]
        public async Task UpdateUserProfile_UserIsAuthenticated_ReturnsNoContent()
        {
            // Arrange
            var mockService = new Mock<IUserProfileService>();
            var userId = "test-uid";
            var userEmail = "test@example.com";
            var userProfileDto = new UserProfileDto { FullName = "Updated Name", Email = "updated@example.com" };
            var controller = CreateController(mockService.Object, userId, userEmail);

            // Act
            var result = await controller.UpdateUserProfile(userProfileDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.UpdateUserProfileAsync(userId, userProfileDto), Times.Once);
        }
    }
}
