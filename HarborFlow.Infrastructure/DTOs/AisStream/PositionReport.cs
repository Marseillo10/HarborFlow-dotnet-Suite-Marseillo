using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs.AisStream
{
    public class PositionReport
    {
        [JsonPropertyName("Cog")]
        public double CourseOverGround { get; set; }

        [JsonPropertyName("Sog")]
        public double SpeedOverGround { get; set; }

        [JsonPropertyName("Latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("Longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("NavigationalStatus")]
        public int NavigationalStatus { get; set; }
    }
}
