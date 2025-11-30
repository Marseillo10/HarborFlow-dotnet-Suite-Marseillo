using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.DTOs;

public class CreateServiceRequestDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Description { get; set; }
    [Required]
    public required string Status { get; set; }
    public int Priority { get; set; }
    public Guid? VesselId { get; set; }
}
