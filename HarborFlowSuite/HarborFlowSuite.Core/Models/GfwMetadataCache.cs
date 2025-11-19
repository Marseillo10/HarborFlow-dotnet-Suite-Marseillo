using System;
using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.Models
{
    public class GfwMetadataCache
    {
        [Key]
        public string Mmsi { get; set; }
        public string Flag { get; set; }
        public double? Length { get; set; }
        public string ImoNumber { get; set; }
        public string ShipName { get; set; }
        public string Callsign { get; set; }
        public string Geartype { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
