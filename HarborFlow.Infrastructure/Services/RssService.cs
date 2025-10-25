using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Infrastructure.Services
{
    public class RssService : IRssService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RssService> _logger;

        public RssService(HttpClient httpClient, ILogger<RssService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<NewsArticle>> FetchNewsAsync(string feedUrl)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(feedUrl);
                using var reader = XmlReader.Create(new System.IO.StringReader(response));
                var feed = SyndicationFeed.Load(reader);

                return feed.Items.Select(item => new NewsArticle
                {
                    Title = item.Title.Text,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Description = item.Summary?.Text,
                    PublishDate = item.PublishDate,
                    Source = feed.Title.Text 
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch or parse RSS feed from {FeedUrl}", feedUrl);
                return Enumerable.Empty<NewsArticle>();
            }
        }
    }
}
