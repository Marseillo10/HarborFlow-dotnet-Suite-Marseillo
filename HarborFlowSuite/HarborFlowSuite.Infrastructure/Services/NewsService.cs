using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "MaritimeNews";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

        private readonly List<string> _rssFeeds = new()
        {
            "https://gcaptain.com/feed/",
            "https://splash247.com/feed/",
            "https://maritime-executive.com/articles.rss"
        };

        public NewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<List<NewsItemDto>> GetNewsAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(CacheKey, out List<NewsItemDto>? cachedNews) && cachedNews != null)
            {
                return cachedNews;
            }

            var allNews = new List<NewsItemDto>();
            var client = _httpClientFactory.CreateClient("NewsClient");

            // Use a standard browser User-Agent to avoid being blocked
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            foreach (var feedUrl in _rssFeeds)
            {
                try
                {
                    var response = await client.GetStringAsync(feedUrl, cancellationToken);
                    var doc = XDocument.Parse(response);

                    // Support both RSS (item) and Atom (entry)
                    var items = doc.Descendants("item").Concat(doc.Descendants("entry")).Select(item =>
                    {
                        var title = item.Element("title")?.Value ?? "No Title";
                        var link = item.Element("link")?.Value ?? item.Element(XName.Get("link", "http://www.w3.org/2005/Atom"))?.Attribute("href")?.Value ?? string.Empty;
                        var pubDateStr = item.Element("pubDate")?.Value ?? item.Element("published")?.Value ?? item.Element("updated")?.Value;
                        var description = item.Element("description")?.Value ?? item.Element("summary")?.Value ?? item.Element("content")?.Value ?? string.Empty;

                        return new NewsItemDto
                        {
                            Title = title,
                            Link = link,
                            PublishDate = DateTime.TryParse(pubDateStr, out var date) ? date : DateTime.UtcNow,
                            Source = GetSourceFromUrl(feedUrl),
                            Summary = StripHtml(description),
                            ImageUrl = GetImageUrl(item)
                        };
                    });

                    allNews.AddRange(items);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching feed {feedUrl}: {ex.Message}");
                }
            }

            var sortedNews = allNews.OrderByDescending(n => n.PublishDate).ToList();
            _cache.Set(CacheKey, sortedNews, CacheDuration);

            return sortedNews;
        }

        private string GetSourceFromUrl(string url)
        {
            if (url.Contains("gcaptain")) return "gCaptain";
            if (url.Contains("splash247")) return "Splash247";
            if (url.Contains("maritime-executive")) return "Maritime Executive";
            return "Maritime News";
        }

        private string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            // Decode HTML entities and strip tags
            var decoded = System.Net.WebUtility.HtmlDecode(input);
            return System.Text.RegularExpressions.Regex.Replace(decoded, "<.*?>", String.Empty).Trim();
        }

        private string? GetImageUrl(XElement item)
        {
            // Try media:content
            var mediaContent = item.Elements().FirstOrDefault(e => e.Name.LocalName == "content" && e.Name.NamespaceName.Contains("media"));
            if (mediaContent != null) return mediaContent.Attribute("url")?.Value;

            // Try enclosure
            var enclosure = item.Element("enclosure");
            if (enclosure != null) return enclosure.Attribute("url")?.Value;

            // Try extracting from description/content if it contains an img tag
            var description = item.Element("description")?.Value ?? item.Element("content")?.Value;
            if (!string.IsNullOrEmpty(description))
            {
                var match = System.Text.RegularExpressions.Regex.Match(description, "<img.+?src=[\"'](.+?)[\"']");
                if (match.Success) return match.Groups[1].Value;
            }

            return null;
        }
    }
}
