
using HarborFlow.Application.Services;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.Services
{
    public class AuthServiceTests
    {
        private HarborFlowDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Unique name for each test
                .Options;
            var context = new HarborFlowDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private ILogger<AuthService> GetLogger()
        {
            return new Mock<ILogger<AuthService>>().Object;
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateNewUser()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());
            var username = "testuser";
            var password = "password123";
            var email = "test@example.com";
            var fullName = "Test User";

            // Act
            var result = await authService.RegisterAsync(username, password, email, fullName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, result.PasswordHash));

            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());
            var username = "testuser";
            var password = "password123";
            await authService.RegisterAsync(username, password, "test@example.com", "Test User");

            // Act
            var result = await authService.AuthenticateAsync(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());
            var username = "testuser";
            var password = "password123";
            await authService.RegisterAsync(username, password, "test@example.com", "Test User");

            // Act
            var result = await authService.AuthenticateAsync(username, "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_WithNonExistentUser_ShouldReturnNull()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());

            // Act
            var result = await authService.AuthenticateAsync("nonexistentuser", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithExistingUser_ShouldReturnTrue()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());
            var username = "testuser";
            await authService.RegisterAsync(username, "password123", "test@example.com", "Test User");

            // Act
            var result = await authService.UserExistsAsync(username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithNonExistentUser_ShouldReturnFalse()
        {
            // Arrange
            var context = GetDbContext();
            var authService = new AuthService(context, GetLogger());

            // Act
            var result = await authService.UserExistsAsync("nonexistentuser");

            // Assert
            Assert.False(result);
        }
    }
}
