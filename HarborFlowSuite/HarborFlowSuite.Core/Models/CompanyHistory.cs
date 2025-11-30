using System;

namespace HarborFlowSuite.Core.Models
{
    public class CompanyHistory
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Action { get; set; } = string.Empty; // "Created", "Updated"
        public string? ChangedBy { get; set; } // UserId or Name
        public string? ChangeDetails { get; set; } // JSON or text description
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Company? Company { get; set; }
    }
}
