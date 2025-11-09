using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Core.Models
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
