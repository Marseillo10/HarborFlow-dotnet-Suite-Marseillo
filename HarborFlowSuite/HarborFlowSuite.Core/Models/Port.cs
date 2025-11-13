using System.Text.Json.Serialization;

namespace HarborFlowSuite.Core.Models
{
    public class Port
    {
        [JsonPropertyName("CITY")]
        public string City { get; set; }

        [JsonPropertyName("STATE")]
        public string State { get; set; }

        [JsonPropertyName("COUNTRY")]
        public string Country { get; set; }

        [JsonPropertyName("LATITUDE")]
        public double Latitude { get; set; }

        [JsonPropertyName("LONGITUDE")]
        public double Longitude { get; set; }

        public string Name => City;
    }
}
