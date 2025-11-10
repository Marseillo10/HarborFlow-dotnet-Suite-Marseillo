namespace HarborFlowSuite.Core.Models
{
    public class PositionReport
    {
        public int UserID { get; set; } // MMSI
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TrueHeading { get; set; }
        public double Sog { get; set; } // Speed Over Ground
    }
}
