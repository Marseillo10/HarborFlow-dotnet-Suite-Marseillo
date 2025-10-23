
using System;
using System.Collections.Generic;

namespace HarborFlow.Core.Models
{
    public enum RequestStatus
    {
        Submitted,
        UnderReview,
        Approved,
        Rejected,
        InProgress,
        Completed,
        Cancelled
    }

    public class ServiceRequest : ICloneable
    {
        public Guid RequestId { get; set; }
        public string VesselImo { get; set; } = string.Empty;
        public Guid RequestedBy { get; set; }
        public ServiceType ServiceType { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Documents { get; set; } = new(); // Simplified for now
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public object Clone()
        {
            var clone = (ServiceRequest)this.MemberwiseClone();
            // Deep copy collections if necessary
            clone.Documents = new List<string>(this.Documents);
            return clone;
        }
    }
}
