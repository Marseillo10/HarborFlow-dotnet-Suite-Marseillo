using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Core.Models
{
    public class AisMessage
    {
        public string MessageType { get; set; }
        public AisMessageContent Message { get; set; }

        public class ShipStaticDataMessage
        {
            public int UserID { get; set; }
            public int Type { get; set; }
            public string Name { get; set; }
            public int ImoNumber { get; set; }
            public string CallSign { get; set; }
            public DimensionData Dimension { get; set; }
            public string Destination { get; set; }
            public EtaData Eta { get; set; }
            public double MaximumStaticDraught { get; set; }

            public class DimensionData
            {
                public int A { get; set; }
                public int B { get; set; }
                public int C { get; set; }
                public int D { get; set; }
            }

            public class EtaData
            {
                public int Month { get; set; }
                public int Day { get; set; }
                public int Hour { get; set; }
                public int Minute { get; set; }
            }
        }

        public class PositionReportMessage
        {
            public int UserID { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double TrueHeading { get; set; }
            public double Sog { get; set; }
            public int NavigationalStatus { get; set; }
        }

        public class StandardClassBPositionReportMessage
        {
            public int UserID { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double TrueHeading { get; set; }
            public double Sog { get; set; }
        }
    }

    public class AisMessageContent
    {
        public AisMessage.PositionReportMessage PositionReport { get; set; }
        public AisMessage.ShipStaticDataMessage ShipStaticData { get; set; }
        public AisMessage.StandardClassBPositionReportMessage StandardClassBPositionReport { get; set; }
    }
}
