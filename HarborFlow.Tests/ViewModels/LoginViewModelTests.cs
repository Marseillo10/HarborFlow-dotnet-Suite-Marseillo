using FluentAssertions;
using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
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
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            _viewModel = new LoginViewModel(_authServiceMock.Object, _windowManagerMock.Object, _mainWindowViewModelMock.Object);
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
        public async Task LoginCommand_ShouldCallShowMainWindow_WhenLoginIsSuccessful()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "password";
            _authServiceMock.Setup(s => s.LoginAsync("test", "password"))
                .ReturnsAsync(new User());

            // Act
            await (_viewModel.LoginCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _windowManagerMock.Verify(w => w.ShowMainWindow(), Times.Once);
        }

        [Fact]
        public async Task LoginCommand_ShouldSetErrorMessage_WhenLoginFails()
        {
            // Arrange
            _viewModel.Username = "test";
            _viewModel.Password = "wrongpassword";
            _authServiceMock.Setup(s => s.LoginAsync("test", "wrongpassword"))
                .ReturnsAsync((User)null);

            // Act
            await (_viewModel.LoginCommand as AsyncRelayCommand).ExecuteAsync(null);

            // Assert
            _viewModel.ErrorMessage.Should().NotBeNullOrEmpty();
            _windowManagerMock.Verify(w => w.ShowMainWindow(), Times.Never);
        }
    }
}
