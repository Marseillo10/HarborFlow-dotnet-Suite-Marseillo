
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HarborFlow.Core.Interfaces;

namespace HarborFlow.Infrastructure.Services
{
    public class RssFeedManager : IRssFeedManager
    {
        private const string FeedsFilePath = "rss-feeds.json";

        public List<string> GetFeedUrls()
        {
            if (!File.Exists(FeedsFilePath))
            {
                return new List<string>();
            }

            var json = File.ReadAllText(FeedsFilePath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}
