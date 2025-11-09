using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<User>? Users { get; set; }
        public ICollection<Vessel>? Vessels { get; set; }
    }
}