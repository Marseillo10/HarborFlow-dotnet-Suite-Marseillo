using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Infrastructure.Services;
using Xunit;

namespace HarborFlow.Backend.Tests.Services
{
    public class RssFeedManagerTests
    {
        private const string TestFileName = "rss-feeds-test.json";

        public RssFeedManagerTests()
        {
            // Clean up any leftover files before a test run
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [Fact]
        public void GetFeedUrls_WhenFileExists_ReturnsUrls()
        {
            // Arrange
            var expectedUrls = new List<string> { "http://test.com/feed1", "http://test.com/feed2" };
            var json = JsonSerializer.Serialize(expectedUrls);
            File.WriteAllText(TestFileName, json);

            // The service is hardcoded to "rss-feeds.json", so we can't inject the name.
            // We will temporarily rename our test file to what the service expects.
            File.Move(TestFileName, "rss-feeds.json");

            var manager = new RssFeedManager();

            try
            {
                // Act
                var result = manager.GetFeedUrls();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(expectedUrls);
            }
            finally
            {
                // Clean up
                if (File.Exists("rss-feeds.json"))
                {
                    File.Delete("rss-feeds.json");
                }
            }
        }

        [Fact]
        public void GetFeedUrls_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            // Arrange
            // Ensure the file does not exist
            if (File.Exists("rss-feeds.json"))
            {
                File.Delete("rss-feeds.json");
            }
            var manager = new RssFeedManager();

            // Act
            var result = manager.GetFeedUrls();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
