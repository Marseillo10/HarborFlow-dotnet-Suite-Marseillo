using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string PrimaryContactEmail { get; set; } = string.Empty;
        public string? BillingAddress { get; set; }
        public HarborFlowSuite.Core.Enums.SubscriptionTier SubscriptionTier { get; set; } = HarborFlowSuite.Core.Enums.SubscriptionTier.Free;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Vessel>? Vessels { get; set; }
        public ICollection<CompanyHistory>? Histories { get; set; }
    }
}