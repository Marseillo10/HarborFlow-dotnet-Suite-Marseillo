using FluentAssertions;
using HarborFlow.Application.Interfaces;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Moq;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IVesselTrackingService> _vesselTrackingServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly SessionContext _sessionContext;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly MapViewModel _mapViewModel;
        private readonly ServiceRequestViewModel _serviceRequestViewModel;
        private readonly VesselManagementViewModel _vesselManagementViewModel;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _sessionContext = new SessionContext();
            
            _mapViewModel = new MapViewModel(_vesselTrackingServiceMock.Object, _notificationServiceMock.Object);
            _dashboardViewModel = new DashboardViewModel(_portServiceManagerMock.Object, _vesselTrackingServiceMock.Object, _sessionContext);
            _serviceRequestViewModel = new ServiceRequestViewModel(_portServiceManagerMock.Object, _windowManagerMock.Object, _sessionContext);
            _vesselManagementViewModel = new VesselManagementViewModel(_vesselTrackingServiceMock.Object, _windowManagerMock.Object);

            _viewModel = new MainWindowViewModel(
                _sessionContext,
                _notificationServiceMock.Object,
                _windowManagerMock.Object,
                _dashboardViewModel,
                _mapViewModel,
                _serviceRequestViewModel,
                _vesselManagementViewModel);
        }

        [Fact]
        public void Constructor_ShouldInitializeToDashboardViewModel()
        {
            // Assert
            _viewModel.CurrentViewModel.Should().Be(_dashboardViewModel);
        }

        [Fact]
        public void NavigateToMapCommand_ShouldChangeCurrentViewModelToMapViewModel()
        {
            // Act
            _viewModel.NavigateToMapCommand.Execute(null);

            // Assert
            _viewModel.CurrentViewModel.Should().Be(_mapViewModel);
        }
        
        [Fact]
        public void NavigateToVesselManagementCommand_ShouldChangeCurrentViewModelToVesselManagementViewModel()
        {
            // Act
            _viewModel.NavigateToVesselManagementCommand.Execute(null);

            // Assert
            _viewModel.CurrentViewModel.Should().Be(_vesselManagementViewModel);
        }

        [Fact]
        public void NavigateToServiceRequestCommand_ShouldChangeCurrentViewModelToServiceRequestViewModel()
        {
            // Act
            _viewModel.NavigateToServiceRequestCommand.Execute(null);

            // Assert
            _viewModel.CurrentViewModel.Should().Be(_serviceRequestViewModel);
        }

        [Fact]
        public void LogoutCommand_ShouldCallShowLoginWindow()
        {
            // Act
            _viewModel.LogoutCommand.Execute(null);

            // Assert
            _windowManagerMock.Verify(w => w.ShowLoginWindow(), Times.Once);
        }
    }
}
