using System;

namespace HarborFlowSuite.Core.DTOs
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string PrimaryContactEmail { get; set; } = string.Empty;
        public string? BillingAddress { get; set; }
        public string SubscriptionTier { get; set; } = "Free";
    }
}
