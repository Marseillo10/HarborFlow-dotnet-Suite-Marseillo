using System.ComponentModel.DataAnnotations;

namespace HarborFlowSuite.Core.DTOs;

public class CreateMapBookmarkDto
{
    [Required]
    public required string Name { get; set; }
    [Required]
    public double Latitude { get; set; }
    [Required]
    public double Longitude { get; set; }
}
