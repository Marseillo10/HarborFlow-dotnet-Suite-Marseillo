using System;

namespace HarborFlowSuite.Core.DTOs
{
    public class VesselPositionDto
    {
        public string VesselId { get; set; }
        public string MMSI { get; set; } = string.Empty; // Added MMSI
        public string VesselName { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public string IMO { get; set; } = string.Empty; // Added IMO
        public string VesselStatus { get; set; } = string.Empty; // Added VesselStatus
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Heading { get; set; }
        public decimal Speed { get; set; }
        public DateTime RecordedAt { get; set; }
        public string Destination { get; set; } = string.Empty;
        public DateTime? Eta { get; set; }
        public string NavigationalStatus { get; set; } = string.Empty;
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Draught { get; set; }
    }
}
