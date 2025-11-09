using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.DTOs;

public class CreateVesselDto
{
    [Required]
    public required string Name { get; set; }
    public string? IMO { get; set; }
    [Required]
    public required string Type { get; set; }
}
