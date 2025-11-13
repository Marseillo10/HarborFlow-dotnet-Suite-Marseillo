using System;

namespace HarborFlowSuite.Core.DTOs
{
    public class VesselPositionDto
    {
        public string VesselId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public string IMO { get; set; } = string.Empty; // Added IMO
        public string VesselStatus { get; set; } = string.Empty; // Added VesselStatus
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Heading { get; set; }
        public decimal Speed { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
