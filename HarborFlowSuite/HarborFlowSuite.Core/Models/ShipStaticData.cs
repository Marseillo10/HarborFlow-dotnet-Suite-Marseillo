namespace HarborFlowSuite.Core.Models
{
    public class ShipStaticData
    {
        public int MessageID { get; set; }
        public int RepeatIndicator { get; set; }
        public int UserID { get; set; } // MMSI
        public bool Valid { get; set; }
        public int AisVersion { get; set; }
        public int ImoNumber { get; set; }
        public string CallSign { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public Dimension Dimension { get; set; }
        public int FixType { get; set; }
        public Eta Eta { get; set; }
        public double MaximumStaticDraught { get; set; }
        public string Destination { get; set; }
        public bool Dte { get; set; }
        public bool Spare { get; set; }
    }

    public class Dimension
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
    }

    public class Eta
    {
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
