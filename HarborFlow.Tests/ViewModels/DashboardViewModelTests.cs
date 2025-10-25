using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class DashboardViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IVesselTrackingService> _vesselTrackingServiceMock;
        private readonly SessionContext _sessionContext;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<DashboardViewModel>> _loggerMock;
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly DashboardViewModel _viewModel;

        public DashboardViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _sessionContext = new SessionContext { CurrentUser = new User { UserId = System.Guid.NewGuid() } };
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<DashboardViewModel>>();
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            _windowManagerMock = new Mock<IWindowManager>();
            _viewModel = new DashboardViewModel(_portServiceManagerMock.Object, _vesselTrackingServiceMock.Object, _sessionContext, _notificationServiceMock.Object, _loggerMock.Object, _mainWindowViewModelMock.Object, _windowManagerMock.Object);
        }

        [Fact]
        public void RefreshCommand_ShouldNotBeNull()
        {
            // Assert
            _viewModel.RefreshCommand.Should().NotBeNull();
        }

        [Fact]
        public async Task LoadDataAsync_ShouldSetVesselAndServiceRequestCounts()
        {
            // Arrange
            var vessels = new List<Vessel> { new Vessel(), new Vessel() };
            var serviceRequests = new List<ServiceRequest>
            {
                new ServiceRequest { Status = RequestStatus.Submitted },
                new ServiceRequest { Status = RequestStatus.InProgress },
                new ServiceRequest { Status = RequestStatus.Completed }
            };

            _vesselTrackingServiceMock.Setup(s => s.GetAllVesselsAsync()).ReturnsAsync(vessels);
            _portServiceManagerMock.Setup(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser)).ReturnsAsync(serviceRequests);

            // Act
            await _viewModel.LoadDataAsync();

            // Assert
            _viewModel.VesselCount.Should().Be(2);
            _viewModel.ActiveServiceRequestCount.Should().Be(2);
        }

        [Fact]
        public async Task LoadDataAsync_ShouldPopulateRecentVesselsAndServiceRequests()
        {
            // Arrange
            var vessels = new List<Vessel>
            {
                new Vessel { UpdatedAt = System.DateTime.Now.AddDays(-1) },
                new Vessel { UpdatedAt = System.DateTime.Now },
                new Vessel { UpdatedAt = System.DateTime.Now.AddDays(-2) }
            };
            var serviceRequests = new List<ServiceRequest>
            {
                new ServiceRequest { RequestDate = System.DateTime.Now.AddDays(-1) },
                new ServiceRequest { RequestDate = System.DateTime.Now },
                new ServiceRequest { RequestDate = System.DateTime.Now.AddDays(-2) }
            };

            _vesselTrackingServiceMock.Setup(s => s.GetAllVesselsAsync()).ReturnsAsync(vessels);
            _portServiceManagerMock.Setup(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser)).ReturnsAsync(serviceRequests);

            // Act
            await _viewModel.LoadDataAsync();

            // Assert
            _viewModel.RecentVessels.Should().HaveCount(3);
            _viewModel.RecentVessels.First().UpdatedAt.Should().Be(vessels[1].UpdatedAt);
            _viewModel.RecentServiceRequests.Should().HaveCount(3);
            _viewModel.RecentServiceRequests.First().RequestDate.Should().Be(serviceRequests[1].RequestDate);
        }

        [Fact]
        public async Task LoadDataAsync_ShouldShowNotificationOnError()
        {
            // Arrange
            _vesselTrackingServiceMock.Setup(s => s.GetAllVesselsAsync()).ThrowsAsync(new System.Exception());

            // Act
            await _viewModel.LoadDataAsync();

            // Assert
            _notificationServiceMock.Verify(n => n.ShowNotification(It.IsAny<string>(), NotificationType.Error), Times.Once);
        }
    }
}
