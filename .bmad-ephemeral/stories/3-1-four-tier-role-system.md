# Story 3.1: Four-Tier Role System Implementation

Status: done

## Description
As a system administrator, I want a defined role-based access control system so that users can be assigned specific roles (System Admin, Port Authority, Vessel Agent, Guest) with corresponding access levels.

## Acceptance Criteria
- [x] Four roles (SystemAdmin, PortAuthority, VesselAgent, Guest) are defined in the system.
- [x] New users are automatically assigned the 'Guest' role upon registration.
- [x] The assigned role is correctly reflected in the user's JWT token as a claim.
- [x] The system supports manual role assignment (verified via database update).

## Dev Agent Record

### Context Reference
Implemented as part of Epic 3: Role-Based Access Control (RBAC) System.

### Agent Model Used
{{agent_model_name_version}}

### Debug Log References
- Fixed `InvalidOperationException` regarding pending model changes by adding `AddRolesDbSet` migration.
- Verified JWT claims using manual inspection.

### Completion Notes List
- Defined role constants in `HarborFlowSuite.Shared/Constants/UserRole.cs`.
- Created `Role` entity and added `DbSet<Role>` to `ApplicationDbContext`.
- Updated `AuthService.RegisterUserAsync` to:
    - Check for and create the `Guest` role if missing.
    - Assign the `Guest` role to new users.
    - Set the `role` custom claim in Firebase Authentication.
- Updated `FirebaseAuthenticationStateProvider` to parse the `role` claim from JWTs.
- Verified implementation by manually promoting a user to `SystemAdmin` and confirming the token claim.

### File List
- HarborFlowSuite/HarborFlowSuite.Shared/Constants/UserRole.cs
- HarborFlowSuite/HarborFlowSuite.Core/Models/Role.cs
- HarborFlowSuite/HarborFlowSuite.Infrastructure/Persistence/ApplicationDbContext.cs
- HarborFlowSuite/HarborFlowSuite.Infrastructure/Services/AuthService.cs
- HarborFlowSuite/HarborFlowSuite.Client/Providers/FirebaseAuthenticationStateProvider.cs
- HarborFlowSuite/HarborFlowSuite.Infrastructure/HarborFlowSuite.Infrastructure.csproj
- HarborFlowSuite/HarborFlowSuite.Shared/HarborFlowSuite.Shared.csproj
