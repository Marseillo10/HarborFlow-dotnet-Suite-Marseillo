using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Wpf.Commands;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class NewsViewModelTests
    {
        private readonly Mock<IRssService> _rssServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<NewsViewModel>> _loggerMock;
        private readonly NewsViewModel _viewModel;

        public NewsViewModelTests()
        {
            _rssServiceMock = new Mock<IRssService>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<NewsViewModel>>();

            var mockConfSection = new Mock<IConfigurationSection>();
            var feeds = new List<string> { "http://test.com/feed" };
            mockConfSection.Setup(s => s.Get<List<string>>()).Returns(feeds);
            _configurationMock.Setup(c => c.GetSection("RssFeeds")).Returns(mockConfSection.Object);

            _viewModel = new NewsViewModel(
                _rssServiceMock.Object,
                _configurationMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task LoadNewsCommand_ShouldPopulateArticlesCollection()
        {
            // Arrange
            var articles = new List<NewsArticle>
            {
                new NewsArticle { Title = "Test Article 1" },
                new NewsArticle { Title = "Test Article 2" }
            };
            _rssServiceMock.Setup(s => s.FetchNewsAsync(It.IsAny<string>()))
                           .ReturnsAsync(articles);

            // Act
                        await ((AsyncRelayCommand)_viewModel.LoadNewsCommand).ExecuteAsync(null);

            // Assert
            _viewModel.Articles.Should().NotBeEmpty();
            _viewModel.Articles.Count.Should().Be(2);
            _viewModel.Articles.First().Title.Should().Be("Test Article 1");
        }
    }
}