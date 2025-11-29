using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class Vessel : Interfaces.IMustHaveCompany
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MMSI { get; set; } = string.Empty;
        public string ImoNumber { get; set; } = string.Empty;
        public string VesselType { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Company? Company { get; set; }
        public ICollection<VesselPosition>? VesselPositions { get; set; }
    }
}