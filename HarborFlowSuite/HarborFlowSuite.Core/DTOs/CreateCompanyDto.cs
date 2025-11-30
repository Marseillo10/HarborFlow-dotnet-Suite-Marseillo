using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.DTOs;

public class CreateCompanyDto
{
    [Required]
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    [Required]
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string? BillingAddress { get; set; }
    public HarborFlowSuite.Core.Enums.SubscriptionTier SubscriptionTier { get; set; } = HarborFlowSuite.Core.Enums.SubscriptionTier.Free;
}
