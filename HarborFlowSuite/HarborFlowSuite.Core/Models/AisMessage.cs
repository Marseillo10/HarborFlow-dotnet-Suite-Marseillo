using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Core.Models
{
    public class AisMessage
    {
        public string MessageType { get; set; }
        public AisMessageContent Message { get; set; }
    }

    public class AisMessageContent
    {
        public PositionReport PositionReport { get; set; }
        public ShipStaticData ShipStaticData { get; set; }
    }
}
