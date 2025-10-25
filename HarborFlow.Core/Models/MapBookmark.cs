using System;
using System.ComponentModel.DataAnnotations;

namespace HarborFlow.Core.Models
{
    public class MapBookmark
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        public double North { get; set; }
        public double South { get; set; }
        public double East { get; set; }
        public double West { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
