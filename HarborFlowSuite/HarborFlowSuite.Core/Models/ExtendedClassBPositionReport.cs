namespace HarborFlowSuite.Core.Models
{
    public class ExtendedClassBPositionReport
    {
        public int MessageID { get; set; }
        public int RepeatIndicator { get; set; }
        public int UserID { get; set; }
        public bool Valid { get; set; }
        public int Spare1 { get; set; }
        public double Sog { get; set; }
        public bool PositionAccuracy { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Cog { get; set; }
        public int TrueHeading { get; set; }
        public int Timestamp { get; set; }
        public int Spare2 { get; set; }
        public string Name { get; set; }
        public int Type { get; set; } // This is the vessel type
        public Dimension Dimension { get; set; }
        public int FixType { get; set; }
        public bool Raim { get; set; }
        public bool Dte { get; set; }
        public bool AssignedMode { get; set; }
        public int Spare3 { get; set; }
    }
}
