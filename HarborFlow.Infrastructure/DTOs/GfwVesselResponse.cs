using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs
{
    // NOTE: This DTO is based on assumptions from the Global Fishing Watch API documentation.
    // The actual JSON structure may differ and this class may need adjustments.

    public class GfwVessel
    {
        [JsonPropertyName("id")]
        public string? VesselId { get; set; }

        [JsonPropertyName("shipname")]
        public string? ShipName { get; set; }

        [JsonPropertyName("mmsi")]
        public string? Mmsi { get; set; }

        [JsonPropertyName("imo")]
        public string? Imo { get; set; }

        [JsonPropertyName("flag")]
        public string? Flag { get; set; }

        [JsonPropertyName("vesselType")]
        public string? VesselType { get; set; }
    }

    public class GfwVesselResponse
    {
        [JsonPropertyName("entries")]
        public List<GfwVessel>? Entries { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
