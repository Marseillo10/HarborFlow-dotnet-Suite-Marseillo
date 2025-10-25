using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using System.Threading;
using Xunit;
using System.Linq;
using HarborFlow.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace HarborFlow.Tests.Services
{
    public class RssServiceTests
    {
        [Fact]
        public async Task FetchNewsAsync_ShouldReturnListOfNewsArticles()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(@"<rss version='2.0'>
                    <channel>
                        <title>Test Feed</title>
                        <item>
                            <title>Test Article</title>
                            <link>http://example.com/article</link>
                            <description>Test Description</description>
                            <pubDate>Sat, 25 Oct 2025 12:00:00 GMT</pubDate>
                        </item>
                    </channel>
                </rss>")
            };

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);
            var loggerMock = new Mock<ILogger<RssService>>();
            var rssService = new RssService(httpClient, loggerMock.Object);

            // Act
            var result = await rssService.FetchNewsAsync("http://example.com/feed");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var article = result.First();
            article.Title.Should().Be("Test Article");
            article.Link.Should().Be("http://example.com/article");
            article.Description.Should().Be("Test Description");
            article.PublishDate.Should().Be(new System.DateTimeOffset(2025, 10, 25, 12, 0, 0, System.TimeSpan.Zero));
            article.Source.Should().Be("Test Feed");
        }
    }
}