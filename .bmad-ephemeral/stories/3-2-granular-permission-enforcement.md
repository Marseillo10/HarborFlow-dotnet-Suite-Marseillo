# Story 3.2: Granular Permission Enforcement

Status: completed

## Description
As a developer, I want to implement a granular permission system so that access control is based on specific capabilities (e.g., `CanManageUsers`) rather than just roles. This allows for more flexible and maintainable security policies.

## Acceptance Criteria
- [x] A central definition of all system permissions exists.
- [x] A mapping between Roles and Permissions is implemented.
- [x] ASP.NET Core Authorization Policies are dynamically registered based on permissions.
- [x] API endpoints (specifically `UsersController`) are protected using these permission-based policies.
- [x] Access is correctly granted or denied based on the user's role and its associated permissions.

## Dev Agent Record

### Context Reference
Part of Epic 3: Role-Based Access Control (RBAC) System.

### Agent Model Used
{{agent_model_name_version}}

### Debug Log References

### Completion Notes List

### File List
