using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarborFlowSuite.Core.Models
{
    public class ApprovalHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ServiceRequestId { get; set; }

        [ForeignKey("ServiceRequestId")]
        public virtual ServiceRequest ServiceRequest { get; set; }

        [Required]
        public Guid ApproverId { get; set; }

        [ForeignKey("ApproverId")]
        public virtual User Approver { get; set; }

        public DateTime ApprovalDate { get; set; }

        public ServiceRequestStatus Status { get; set; }

        public string Comments { get; set; }

        public string Action { get; set; }
        public DateTime ActionAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}