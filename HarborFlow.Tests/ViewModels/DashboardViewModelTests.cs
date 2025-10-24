using FluentAssertions;
using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
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
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly DashboardViewModel _viewModel;

        public DashboardViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _sessionContext = new SessionContext { CurrentUser = new User { UserId = System.Guid.NewGuid() } };
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            _viewModel = new DashboardViewModel(_portServiceManagerMock.Object, _vesselTrackingServiceMock.Object, _sessionContext, _mainWindowViewModelMock.Object);
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
    }
}
