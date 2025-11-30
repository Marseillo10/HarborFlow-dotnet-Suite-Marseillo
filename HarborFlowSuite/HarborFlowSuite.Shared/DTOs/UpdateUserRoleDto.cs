using System;

namespace HarborFlowSuite.Shared.DTOs
{
    public class UpdateUserRoleDto
    {
        public Guid RoleId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
