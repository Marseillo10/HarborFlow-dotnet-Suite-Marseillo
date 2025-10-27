using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Infrastructure.Services
{
    public class RssService : IRssService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RssService> _logger;

        // Keyword list for maritime relevance filtering
        private static readonly List<string> MaritimeKeywords = new List<string>
        {
            // English General
            "maritime", "shipping", "port", "vessel", "ship", "offshore", "cargo", "container", 
            "tanker", "bulk", "sail", "seafarer", "shipyard", "ocean", "logistics", "freight", 
            "imo", "suez", "panama canal", "charter", "maersk", "cosco", "cma cgm", "hapag-lloyd",
            "evergreen", "yang ming", "one network", "shipbuilding", "docking", "mooring", "berth",

            // Indonesian General
            "maritim", "pelabuhan", "kapal", "pelayaran", "perkapalan", "logistik", "kelautan", 
            "perikanan", "lepas pantai", "galangan", "pelni", "samudera", "kargo", "tol laut",
            "muatan", "berlayar", "berlabuh", "nakhoda", "abk", "bongkar muat",

            // Logistics & Supply Chain
            "supply chain", "freight rate", "multimoda", 
            "port congestion", "kongesti pelabuhan", "demurrage", "detention",

            // Vessel Types
            "bulker", "lng carrier", "lpg carrier", "ro-ro", "ferry", "cruise", "yacht", "barge", 
            "tongkang", "dredger", "kapal keruk",

            // Legal & Regulatory
            "solas", "marpol", "bill of lading", "konosemen", "incoterms", "admiralty law", 
            "hukum maritim", "coast guard", "penjaga pantai", "bakorkamla", "kplp",

            // Technology & Environment
            "scrubber", "ballast water", 
            "autonomous ship", "kapal otonom", "e-navigation", "green shipping",

            // Geographic
            "strait of malacca", "selat malaka", "strait of hormuz", "selat hormuz", 
            "bab el-mandeb", "south china sea", "laut cina selatan"
        };

        public RssService(HttpClient httpClient, IConfiguration configuration, ILogger<RssService> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsByCategoryAsync(string category)
        {
            var feedUrls = GetFeedUrlsForCategory(category);
            var allArticles = new List<NewsArticle>();

            var fetchTasks = feedUrls.Select(url => FetchAndParseFeedAsync(url));
            var results = await Task.WhenAll(fetchTasks);

            foreach (var articles in results)
            {
                allArticles.AddRange(articles);
            }

            // Apply keyword filter
            var relevantArticles = FilterArticlesByRelevance(allArticles);

            return relevantArticles.OrderByDescending(a => a.PublishDate);
        }

        private List<string> GetFeedUrlsForCategory(string category)
        {
            if (category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var internationalFeeds = _configuration.GetSection("RssFeeds:International").Get<List<string>>() ?? new List<string>();
                var officialFeeds = _configuration.GetSection("RssFeeds:Official").Get<List<string>>() ?? new List<string>();
                var nationalFeeds = _configuration.GetSection("RssFeeds:National").Get<List<string>>() ?? new List<string>();
                return internationalFeeds.Concat(officialFeeds).Concat(nationalFeeds).Distinct().ToList();
            }

            return _configuration.GetSection($"RssFeeds:{category}").Get<List<string>>() ?? new List<string>();
        }

        private async Task<IEnumerable<NewsArticle>> FetchAndParseFeedAsync(string feedUrl)
        {
            try
            {
                _logger.LogInformation("Fetching RSS feed from {FeedUrl}", feedUrl);
                var response = await _httpClient.GetStringAsync(feedUrl);
                
                var settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 1024
                };

                using var reader = XmlReader.Create(new System.IO.StringReader(response), settings);
                SyndicationFeed feed;
                try
                {
                    feed = SyndicationFeed.Load(reader);
                }
                catch (XmlException xmlEx)
                {
                    _logger.LogWarning(xmlEx, "XML parsing failed for feed {FeedUrl}. It might not be a valid RSS/Atom feed.", feedUrl);
                    return Enumerable.Empty<NewsArticle>(); // Return empty for this feed
                }

                return feed.Items.Select(item => new NewsArticle
                {
                    Title = item.Title?.Text,
                    Link = item.Links.FirstOrDefault()?.Uri.ToString(),
                    Description = item.Summary?.Text,
                    PublishDate = item.PublishDate.DateTime,
                    Source = feed.Title?.Text
                }).Where(a => a.Title != null && a.Link != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch or parse RSS feed from {FeedUrl}", feedUrl);
                return Enumerable.Empty<NewsArticle>();
            }
        }

        private IEnumerable<NewsArticle> FilterArticlesByRelevance(IEnumerable<NewsArticle> articles)
        {
            return articles.Where(article => 
            {
                var title = article.Title ?? string.Empty;
                var description = article.Description ?? string.Empty;
                return MaritimeKeywords.Any(keyword => 
                    title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                    description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                                    });
                                }
                            }
                        }