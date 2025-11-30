namespace HarborFlowSuite.Core.DTOs
{
    public class NewsItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
