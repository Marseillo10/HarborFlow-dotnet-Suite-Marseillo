namespace HarborFlowSuite.Shared.DTOs
{
    public class VesselMetadataDto
    {
        public string Flag { get; set; } = string.Empty;
        public double? Length { get; set; }
        public string ImoNumber { get; set; } = string.Empty;
        public string ShipName { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public string Geartype { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime? Eta { get; set; }
        public double? Draught { get; set; }
        public double? Width { get; set; }
        public string? VesselType { get; set; } // Added to support GFW type updates
    }
}
