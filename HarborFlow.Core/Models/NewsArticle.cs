using System;

namespace HarborFlow.Core.Models
{
    public class NewsArticle
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string? Source { get; set; }
        public string? Feed { get; set; }
    }
}