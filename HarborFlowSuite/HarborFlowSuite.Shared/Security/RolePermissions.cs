using System.Collections.Generic;
using HarborFlowSuite.Shared.Constants;

namespace HarborFlowSuite.Shared.Security
{
    public static class RolePermissions
    {
        // Define the inheritance chain: Key inherits from Value
        private static readonly Dictionary<string, string> _roleInheritance = new()
        {
            { UserRole.SystemAdmin, UserRole.PortAuthority },
            { UserRole.PortAuthority, UserRole.VesselAgent },
            { UserRole.VesselAgent, UserRole.Guest }
        };

        // Define ONLY the unique permissions for each role (deltas)
        private static readonly Dictionary<string, HashSet<string>> _rolePermissions = new()
        {
            {
                UserRole.SystemAdmin, new HashSet<string>
                {
                    Permissions.Users.Manage,
                    Permissions.Companies.View,
                    Permissions.Companies.Manage
                }
            },
            {
                UserRole.PortAuthority, new HashSet<string>
                {
                    Permissions.Users.View
                }
            },
            {
                UserRole.VesselAgent, new HashSet<string>
                {
                    Permissions.Dashboard.View,
                    Permissions.Vessels.View,
                    Permissions.Vessels.Manage,
                    Permissions.ServiceRequests.View,
                    Permissions.ServiceRequests.Manage
                }
            },
            {
                UserRole.Guest, new HashSet<string>
                {
                    // Minimal permissions
                }
            }
        };

        public static HashSet<string> GetPermissionsForRole(string role)
        {
            var permissions = new HashSet<string>();
            var currentRole = role;

            // Traverse the inheritance chain up to the root
            // Use a safety counter to prevent infinite loops in case of circular references (though unlikely here)
            int depth = 0;
            const int MaxDepth = 10;

            while (!string.IsNullOrEmpty(currentRole) && depth < MaxDepth)
            {
                if (_rolePermissions.TryGetValue(currentRole, out var rolePerms))
                {
                    foreach (var perm in rolePerms)
                    {
                        permissions.Add(perm);
                    }
                }

                // Move to parent role
                if (!_roleInheritance.TryGetValue(currentRole, out var parentRole))
                {
                    break;
                }
                currentRole = parentRole;
                depth++;
            }

            return permissions;
        }
    }
}
