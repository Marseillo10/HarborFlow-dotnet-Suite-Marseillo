using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class LoginViewModelTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly SessionContext _sessionContext;
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _sessionContext = new SessionContext();
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            _viewModel = new LoginViewModel(_authServiceMock.Object, _windowManagerMock.Object, _sessionContext, _notificationServiceMock.Object, _mainWindowViewModelMock.Object);
        }

        [Fact]
        public void OpenRegisterWindowCommand_ShouldNotBeNull()
        {
            // Assert
            _viewModel.OpenRegisterWindowCommand.Should().NotBeNull();
        }

        [Fact]
        public void LoginCommand_ShouldBeDisabled_WhenUsernameIsEmpty()
        {
            // Arrange
            _viewModel.Username = "";
            _viewModel.Password = "password";

            // Act
            var canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void LoginCommand_ShouldBeDisabled_WhenPasswordIsEmpty()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "";

            // Act
            var canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeFalse();
        }

        [Fact]
        public void LoginCommand_ShouldBeEnabled_WhenUsernameAndPasswordAreProvided()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";

            // Act
            var canExecute = _viewModel.LoginCommand.CanExecute(null);

            // Assert
            canExecute.Should().BeTrue();
        }

        [Fact]
        public async Task LoginCommand_ShouldCallShowMainWindowAndSetSessionContext_WhenLoginIsSuccessful()
        {
            // Arrange
            var user = new User();
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _authServiceMock.Setup(s => s.LoginAsync("test", "password"))
                .ReturnsAsync(user);

            // Act
            await (_viewModel.LoginCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _windowManagerMock.Verify(w => w.ShowMainWindow(), Times.Once);
            _sessionContext.CurrentUser.Should().Be(user);
        }

        [Fact]
        public async Task LoginCommand_ShouldClearPassword_AfterLoginAttempt()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            await (_viewModel.LoginCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _viewModel.Password.Should().BeEmpty();
        }

        [Fact]
        public async Task LoginCommand_ShouldShowNotification_WhenLoginFails()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "wrongpassword";
            _authServiceMock.Setup(s => s.LoginAsync("test", "wrongpassword"))
                .ReturnsAsync((User)null);

            // Act
            await (_viewModel.LoginCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _notificationServiceMock.Verify(n => n.ShowNotification(It.IsAny<string>(), NotificationType.Error), Times.Once);
            _windowManagerMock.Verify(w => w.ShowMainWindow(), Times.Never);
        }
    }
}
