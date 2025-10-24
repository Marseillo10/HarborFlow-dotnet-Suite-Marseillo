using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Validators;
using HarborFlow.Wpf.ViewModels;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class RegisterViewModelTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly RegisterViewModelValidator _validator;
        private readonly RegisterViewModel _viewModel;

        public RegisterViewModelTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _validator = new RegisterViewModelValidator();
            _viewModel = new RegisterViewModel(_authServiceMock.Object, _validator, _notificationServiceMock.Object, _windowManagerMock.Object);
        }

        [Fact]
        public void RegisterCommand_ShouldNotBeNull()
        {
            // Assert
            _viewModel.RegisterCommand.Should().NotBeNull();
        }

        [Fact]
        public void RegisterCommand_ShouldBeDisabled_WhenUsernameIsEmpty()
        {
            // Arrange
            _viewModel.Username = "";
            _viewModel.Password = "password";

            // Act
            var canExecute = _viewModel.RegisterCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void RegisterCommand_ShouldBeDisabled_WhenPasswordIsEmpty()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "";

            // Act
            var canExecute = _viewModel.RegisterCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void RegisterCommand_ShouldBeEnabled_WhenUsernameAndPasswordAreProvided()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";

            // Act
            var canExecute = _viewModel.RegisterCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterCommand_ShouldShowValidationError_ForInvalidData()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "wrongpassword";

            // Act
            await (_viewModel.RegisterCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _notificationServiceMock.Verify(n => n.ShowNotification(It.IsAny<string>(), NotificationType.Error), Times.Once);
        }

        [Fact]
        public async Task RegisterCommand_ShouldShowError_WhenUserExists()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "password";
            _authServiceMock.Setup(s => s.UserExistsAsync("test")).ReturnsAsync(true);

            // Act
            await (_viewModel.RegisterCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _notificationServiceMock.Verify(n => n.ShowNotification("Username already exists.", NotificationType.Error), Times.Once);
        }

        [Fact]
        public async Task RegisterCommand_ShouldRegisterUserAndRaiseEvent_OnSuccess()
        {
            // Arrange
            var registrationSucceeded = false;
            _viewModel.RegistrationSucceeded += (s, e) => registrationSucceeded = true;
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "password";
            _viewModel.Email = "test@test.com";
            _viewModel.FullName = "Test User";
            _authServiceMock.Setup(s => s.UserExistsAsync("test")).ReturnsAsync(false);

            // Act
            await (_viewModel.RegisterCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _authServiceMock.Verify(s => s.RegisterAsync("test", "password", "test@test.com", "Test User"), Times.Once);
            registrationSucceeded.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterCommand_ShouldClearPasswordFields_AfterRegistrationAttempt()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "password";
            _authServiceMock.Setup(s => s.UserExistsAsync("test")).ReturnsAsync(true);

            // Act
            await (_viewModel.RegisterCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _viewModel.Password.Should().BeEmpty();
            _viewModel.ConfirmPassword.Should().BeEmpty();
        }

        [Fact]
        public async Task RegisterCommand_ShouldShowNotification_OnError()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "password";
            _authServiceMock.Setup(s => s.UserExistsAsync("test")).ReturnsAsync(false);
            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new System.Exception());

            // Act
            await (_viewModel.RegisterCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _notificationServiceMock.Verify(n => n.ShowNotification(It.IsAny<string>(), NotificationType.Error), Times.Once);
        }
    }
}
