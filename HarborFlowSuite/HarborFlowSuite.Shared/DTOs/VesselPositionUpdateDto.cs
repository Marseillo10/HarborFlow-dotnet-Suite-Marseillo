using System;

namespace HarborFlowSuite.Shared.DTOs
{
    public class VesselPositionUpdateDto
    {
        public string MMSI { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public double Speed { get; set; }
        public string Name { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public VesselMetadataDto? Metadata { get; set; }
        public Guid? VesselId { get; set; }
    }
}
