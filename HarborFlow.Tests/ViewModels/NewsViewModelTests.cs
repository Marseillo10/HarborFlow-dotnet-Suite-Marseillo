
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.ViewModels;
using Moq;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class NewsViewModelTests
    {
        [Fact]
        public async Task NewsViewModel_ShouldLoadNewsArticles_OnInitialization()
        {
            // Arrange
            var rssServiceMock = new Mock<IRssService>();
            var rssFeedManagerMock = new Mock<IRssFeedManager>();
            var articles = new List<NewsArticle>
            {
                new NewsArticle { Title = "Article 1" },
                new NewsArticle { Title = "Article 2" }
            };
            rssFeedManagerMock.Setup(m => m.GetFeedUrls()).Returns(new List<string> { "http://example.com/feed" });
            rssServiceMock.Setup(s => s.GetNewsAsync(It.IsAny<string>())).ReturnsAsync(articles);

            // Act
            var viewModel = new NewsViewModel(rssServiceMock.Object, rssFeedManagerMock.Object);
            await Task.Delay(100); // Allow async loading to complete

            // Assert
            Assert.NotNull(viewModel.NewsArticles);
            Assert.Equal(2, viewModel.NewsArticles.Count);
            Assert.Equal("Article 1", viewModel.NewsArticles[0].Title);
        }
    }
}
