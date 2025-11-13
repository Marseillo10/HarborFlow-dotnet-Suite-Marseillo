using System;

namespace HarborFlowSuite.Core.DTOs
{
    public class VesselDto
    {
        public Guid Id { get; set; }
        public string Mmsi { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImoNumber { get; set; } = string.Empty;
        public string Country { get; set; }
        public string Type { get; set; }
        public string VesselType { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public VesselPositionDto? CurrentPosition { get; set; }
        public CompanyDto Company { get; set; } = new();
        public VesselMetadataDto Metadata { get; set; } = new();
    }
}
