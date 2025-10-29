using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs.GlobalFishingWatch
{
    public class GfwVesselResponse
    {
        [JsonPropertyName("entries")]
        public List<GfwVessel> Entries { get; set; }
    }

    public class GfwVessel
    {
        [JsonPropertyName("selfReportedInfo")]
        public List<GfwSelfReportedInfo> SelfReportedInfo { get; set; }
    }

    public class GfwSelfReportedInfo
    {
        [JsonPropertyName("shiptype")]
        public string ShipType { get; set; }
    }
}
