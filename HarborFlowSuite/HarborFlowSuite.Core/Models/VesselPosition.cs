using System;

namespace HarborFlowSuite.Core.Models
{
    public class VesselPosition
    {
        public Guid Id { get; set; }
        public Guid VesselId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Heading { get; set; }
        public decimal Speed { get; set; }
        public DateTime RecordedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Vessel? Vessel { get; set; }
    }
}
