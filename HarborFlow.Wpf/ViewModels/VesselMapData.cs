namespace HarborFlow.Wpf.ViewModels
{
    public class VesselMapData
    {
        public string Imo { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Course { get; set; }
        public double? Speed { get; set; }
        public string VesselType { get; set; }
        public string IconUrl { get; set; }
    }
}
