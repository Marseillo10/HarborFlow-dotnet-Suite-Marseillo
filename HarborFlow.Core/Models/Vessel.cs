
using System;
using System.Collections.Generic;

namespace HarborFlow.Core.Models
{
    public enum VesselType
    {
        Cargo,
        Tanker,
        Passenger,
        Fishing,
        PleasureCraft,
        Sailing,
        Tug,
        Other
    }

    public class Vessel : ICloneable
    {
        public string IMO { get; set; } = string.Empty;
        public string Mmsi { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public VesselType VesselType { get; set; }
        public string FlagState { get; set; } = string.Empty;
        public decimal LengthOverall { get; set; }
        public decimal Beam { get; set; }
        public decimal GrossTonnage { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Metadata { get; set; } = string.Empty; // Using string for jsonb, EF Core handles mapping

        public ICollection<VesselPosition> Positions { get; set; } = new List<VesselPosition>();

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
