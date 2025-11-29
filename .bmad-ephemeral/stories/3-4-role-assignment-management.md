# Story 3.4: Role Assignment Management

Status: completed

## Description
As a **System Administrator**, I want to be able to assign roles to users so that I can control their access level within the application.

## Acceptance Criteria
- [ ] **API Endpoint:** Create an endpoint (e.g., `PUT /api/users/{id}/role`) to update a user's role.
- [ ] **Validation:** Ensure only `SystemAdmin` (or users with `Users.Manage` permission) can assign roles.
- [ ] **Self-Protection:** Prevent users from modifying their own role to escalate privileges (optional but recommended).
- [ ] **UI Integration:** Update the User Management UI to allow selecting a role from a dropdown when creating or editing a user.
- [ ] **Verification:** Verify that the user's permissions change immediately (or after token refresh) upon role update.

## Technical Notes
-   The `User` entity already has a `RoleId` or `Role` property.
-   Need to ensure the `Role` claim in the JWT is updated or that the application checks the database if the token is stale (which we partly implemented in 3.3).
-   Consider using a predefined list of roles from `UserRole` constants.

## Dependencies
-   Story 3.1 (Roles) must be complete.
