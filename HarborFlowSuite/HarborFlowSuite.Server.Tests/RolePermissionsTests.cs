using Xunit;
using HarborFlowSuite.Shared.Security;
using HarborFlowSuite.Shared.Constants;
using System.Collections.Generic;

namespace HarborFlowSuite.Server.Tests
{
    public class RolePermissionsTests
    {
        [Fact]
        public void GetPermissionsForRole_SystemAdmin_ShouldInheritAll()
        {
            // Act
            var permissions = RolePermissions.GetPermissionsForRole(UserRole.SystemAdmin);

            // Assert
            Assert.Contains(Permissions.Users.Manage, permissions); // Own
            Assert.Contains(Permissions.Users.View, permissions); // Inherited from PortAuthority
            Assert.Contains(Permissions.Vessels.Manage, permissions); // Inherited from VesselAgent (via PortAuthority)
            Assert.Contains(Permissions.Dashboard.View, permissions); // Inherited from VesselAgent (via PortAuthority)
        }

        [Fact]
        public void GetPermissionsForRole_PortAuthority_ShouldInheritVesselAgent()
        {
            // Act
            var permissions = RolePermissions.GetPermissionsForRole(UserRole.PortAuthority);

            // Assert
            Assert.Contains(Permissions.Users.View, permissions); // Own
            Assert.Contains(Permissions.Vessels.Manage, permissions); // Inherited from VesselAgent
            Assert.DoesNotContain(Permissions.Users.Manage, permissions); // Not inherited
        }

        [Fact]
        public void GetPermissionsForRole_VesselAgent_ShouldHaveBasicPermissions()
        {
            // Act
            var permissions = RolePermissions.GetPermissionsForRole(UserRole.VesselAgent);

            // Assert
            Assert.Contains(Permissions.Vessels.Manage, permissions);
            Assert.DoesNotContain(Permissions.Users.View, permissions);
        }

        [Fact]
        public void GetPermissionsForRole_Guest_ShouldHaveMinimalPermissions()
        {
            // Act
            var permissions = RolePermissions.GetPermissionsForRole(UserRole.Guest);

            // Assert
            Assert.Empty(permissions); // Currently empty
        }
    }
}
