
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;

namespace HarborFlow.Application.Services
{
    public class RssService : IRssService
    {
        private readonly HttpClient _httpClient;

        public RssService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NewsArticle>> GetNewsAsync(string feedUrl)
        {
            try
            {
                var feedContent = await _httpClient.GetStringAsync(feedUrl);
                using (var reader = XmlReader.Create(new System.IO.StringReader(feedContent)))
                {
                    var feed = SyndicationFeed.Load(reader);
                    return feed.Items.Select(item => new NewsArticle
                    {
                        Title = item.Title.Text,
                        Link = item.Links.FirstOrDefault()?.Uri.ToString(),
                        Description = item.Summary.Text,
                        PublishDate = item.PublishDate.DateTime,
                        Source = feed.Title.Text,
                        Feed = feed.Title.Text
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network errors, parsing errors)
                Console.WriteLine($"Error fetching or parsing RSS feed: {ex.Message}");
                return new List<NewsArticle>();
            }
        }
    }
}
