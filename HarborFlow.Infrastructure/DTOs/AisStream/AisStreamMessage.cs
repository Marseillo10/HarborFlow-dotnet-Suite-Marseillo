using System.Text.Json.Serialization;

namespace HarborFlow.Infrastructure.DTOs.AisStream
{
    public class AisStreamMessage
    {
        [JsonPropertyName("MessageType")]
        public string MessageType { get; set; } = string.Empty;

        [JsonPropertyName("MetaData")]
        public MetaData MetaData { get; set; } = new MetaData();

        [JsonPropertyName("Message")]
        public PositionReport Message { get; set; } = new PositionReport();
    }
}
