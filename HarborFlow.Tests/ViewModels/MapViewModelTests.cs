using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class MapViewModelTests
    {
        private readonly Mock<IVesselTrackingService> _vesselTrackingServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<MapViewModel>> _loggerMock;
        private readonly Mock<IBookmarkService> _bookmarkServiceMock;
        private readonly SessionContext _sessionContext;
        private readonly MapViewModel _viewModel;

        public MapViewModelTests()
        {
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<MapViewModel>>();
            _bookmarkServiceMock = new Mock<IBookmarkService>();
            _sessionContext = new SessionContext();

            _vesselTrackingServiceMock.Setup(s => s.TrackedVessels).Returns(new ObservableCollection<Vessel>());
            
            _viewModel = new MapViewModel(
                _vesselTrackingServiceMock.Object, 
                _notificationServiceMock.Object, 
                _loggerMock.Object,
                _bookmarkServiceMock.Object,
                _sessionContext);
        }

        [Fact]
        public void SearchVesselsCommand_ShouldNotBeNull()
        {
            // Assert
            _viewModel.SearchVesselsCommand.Should().NotBeNull();
        }

        [Fact]
        public void SelectSuggestionCommand_ShouldNotBeNull()
        {
            // Assert
            _viewModel.SelectSuggestionCommand.Should().NotBeNull();
        }

        [Fact]
        public async Task SearchVesselsAsync_ShouldPopulateSearchResults()
        {
            // Arrange
            var vessels = new List<Vessel> { new Vessel(), new Vessel() };
            _vesselTrackingServiceMock.Setup(s => s.SearchVesselsAsync(It.IsAny<string>())).ReturnsAsync(vessels);

            // Act
            await _viewModel.SearchVesselsAsync();

            // Assert
            _viewModel.SearchResults.Should().HaveCount(2);
        }
    }
}