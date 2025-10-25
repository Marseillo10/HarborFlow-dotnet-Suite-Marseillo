using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
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
        private readonly MapViewModel _viewModel;

        public MapViewModelTests()
        {
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<MapViewModel>>();
            _vesselTrackingServiceMock.Setup(s => s.TrackedVessels).Returns(new ObservableCollection<Vessel>());
            _viewModel = new MapViewModel(_vesselTrackingServiceMock.Object, _notificationServiceMock.Object, _loggerMock.Object);
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

        [Fact]
        public async Task UpdateSuggestionsAsync_ShouldPopulateSuggestions()
        {
            // Arrange
            var vessels = new List<Vessel> { new Vessel { Name = "Vessel1" }, new Vessel { Name = "Vessel2" } };
            _vesselTrackingServiceMock.Setup(s => s.SearchVesselsAsync(It.IsAny<string>())).ReturnsAsync(vessels);
            _viewModel.SearchTerm = "vessel";

            // Act
            await Task.Delay(100); // Allow for the async method to run

            // Assert
            _viewModel.Suggestions.Should().HaveCount(2);
            _viewModel.Suggestions.First().Should().Be("Vessel1");
        }

        [Fact]
        public void UpdateFilteredVessels_ShouldFilterVesselsOnMap()
        {
            // Arrange
            var vessels = new ObservableCollection<Vessel>
            {
                new Vessel { VesselType = VesselType.Cargo },
                new Vessel { VesselType = VesselType.Tanker },
                new Vessel { VesselType = VesselType.Cargo }
            };
            _vesselTrackingServiceMock.Setup(s => s.TrackedVessels).Returns(vessels);
            _viewModel.SelectedVesselTypeFilter = VesselType.Cargo;

            // Act
            // The method is called automatically when the filter changes

            // Assert
            _viewModel.FilteredVesselsOnMap.Should().HaveCount(2);
        }
    }
}
