using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs.AisStream
{
    public class MetaData
    {
        [JsonPropertyName("MMSI")]
        public int Mmsi { get; set; }

        [JsonPropertyName("IMO")]
        public int Imo { get; set; }

        [JsonPropertyName("NAME")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("CALLSIGN")]
        public string CallSign { get; set; } = string.Empty;

        [JsonPropertyName("TYPE")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("FLAG")]
        public string Flag { get; set; } = string.Empty;
    }
}
