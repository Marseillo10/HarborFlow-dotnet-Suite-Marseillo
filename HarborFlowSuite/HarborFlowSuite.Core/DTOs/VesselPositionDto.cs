using System;

namespace HarborFlowSuite.Core.DTOs
{
    public class VesselPositionDto
    {
        public Guid VesselId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Heading { get; set; }
        public decimal Speed { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
