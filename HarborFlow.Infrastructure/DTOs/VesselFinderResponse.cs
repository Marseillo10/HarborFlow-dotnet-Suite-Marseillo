
using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs
{
    public class VesselFinderResponse
    {
        [JsonPropertyName("AIS")]
        public AisData? Ais { get; set; }
    }
}
