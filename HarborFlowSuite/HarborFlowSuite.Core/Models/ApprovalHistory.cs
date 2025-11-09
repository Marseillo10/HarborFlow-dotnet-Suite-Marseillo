using System;

namespace HarborFlowSuite.Core.Models
{
    public class ApprovalHistory
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public Guid ApproverId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateTime ActionAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public ServiceRequest? ServiceRequest { get; set; }
        public User? Approver { get; set; }
    }
}
