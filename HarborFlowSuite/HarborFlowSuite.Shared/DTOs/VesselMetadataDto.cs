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
    }
}
