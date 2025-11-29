# Story 3.5: Permission Inheritance

Status: completed

## Description
As a **Developer**, I want to implement permission inheritance so that high-level roles (like `SystemAdmin`) automatically inherit all permissions from lower-level roles (like `CompanyAdmin`), reducing the need to manually map every single permission to every role.

## Acceptance Criteria
- [ ] **Inheritance Structure:** Define a hierarchy or inclusion list for roles (e.g., `SystemAdmin` includes `CompanyAdmin`).
- [ ] **Recursive Resolution:** When checking permissions for a role, the system should check permissions explicitly assigned to that role AND permissions assigned to any inherited roles.
- [ ] **Optimization:** Ensure this resolution is efficient (cached) and doesn't impact performance on every request.
- [ ] **Verification:** Verify that a `SystemAdmin` has `CompanyAdmin` permissions even if they are not explicitly linked in the `RolePermissions` table.

## Technical Notes
-   This can be implemented in `RolePermissions.cs` or a database table `RoleInheritance`.
-   Current implementation uses a static dictionary in `RolePermissions.cs`. We can modify `GetPermissionsForRole` to recursively fetch permissions.

## Dependencies
-   Story 3.2 (Granular Permissions) must be complete.
