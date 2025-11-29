using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class User : Interfaces.IMustHaveCompany
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? RoleId { get; set; }
        public string FirebaseUid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Company Company { get; set; }
        public Role Role { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ServiceRequest> ServiceRequests { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<ApprovalHistory> ApprovalHistories { get; set; }
    }
}