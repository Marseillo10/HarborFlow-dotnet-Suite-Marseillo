using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.DTOs;

public class CreateCompanyDto
{
    [Required]
    public required string Name { get; set; }
}
