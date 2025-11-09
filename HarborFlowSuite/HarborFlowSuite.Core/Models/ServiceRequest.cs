using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class ServiceRequest
    {
        public Guid Id { get; set; }
        public Guid RequestorUserId { get; set; }
        public Guid? AssignedOfficerUserId { get; set; }
        public Guid? VesselId { get; set; }
        public Guid CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? Requester { get; set; }
        public User? AssignedOfficer { get; set; }
        public Vessel? Vessel { get; set; }
        public Company? Company { get; set; }
        public ICollection<ApprovalHistory>? ApprovalHistories { get; set; }
    }
}