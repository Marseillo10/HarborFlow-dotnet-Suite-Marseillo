
using System.Net.Http;
using System.Threading.Tasks;
using HarborFlow.Application.Services;
using Moq;
using Moq.Protected;
using System.Threading;
using Xunit;
using System.Linq;

namespace HarborFlow.Tests.Services
{
    public class RssServiceTests
    {
        [Fact]
        public async Task GetNewsAsync_ShouldReturnListOfNewsArticles()
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
            var rssService = new RssService(httpClient);

            // Act
            var result = await rssService.GetNewsAsync("http://example.com/feed");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var article = result.First();
            Assert.Equal("Test Article", article.Title);
            Assert.Equal("http://example.com/article", article.Link);
            Assert.Equal("Test Description", article.Description);
            Assert.Equal(new System.DateTime(2025, 10, 25, 12, 0, 0, System.DateTimeKind.Utc), article.PublishDate);
            Assert.Equal("Test Feed", article.Source);
        }
    }
}
