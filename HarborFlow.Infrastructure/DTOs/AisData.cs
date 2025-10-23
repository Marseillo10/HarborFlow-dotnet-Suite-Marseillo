
using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs
{
    public class AisData
    {
        [JsonPropertyName("MMSI")]
        public int Mmsi { get; set; }

        [JsonPropertyName("TIMESTAMP")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("LATITUDE")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("LONGITUDE")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("COURSE")]
        public double Course { get; set; }

        [JsonPropertyName("SPEED")]
        public double Speed { get; set; }

        [JsonPropertyName("HEADING")]
        public int Heading { get; set; }

        [JsonPropertyName("NAVSTAT")]
        public int Navstat { get; set; }

        [JsonPropertyName("IMO")]
        public int Imo { get; set; }

        [JsonPropertyName("NAME")]
        public string? Name { get; set; }
    }
}
