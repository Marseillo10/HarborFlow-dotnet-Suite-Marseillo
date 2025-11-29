using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarborFlowSuite.Core.Models
{
    public class ServiceRequest : Interfaces.IMustHaveCompany
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public ServiceRequestStatus Status { get; set; }

        public DateTime RequestDate { get; set; }

        public Guid? RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public virtual User Requester { get; set; }

        public Guid? VesselId { get; set; }

        [ForeignKey("VesselId")]
        public virtual Vessel Vessel { get; set; }

        public virtual ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();

        public DateTime UpdatedAt { get; set; }
        public Guid? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Priority { get; set; }
        public DateTime RequestedAt { get; set; }
        public Guid? AssignedOfficerId { get; set; }
        [ForeignKey("AssignedOfficerId")]
        public virtual User AssignedOfficer { get; set; }
    }
}
