# Story 3.7: Decentralized Company Management

Status: in-progress

## Description
As a **Company Administrator**, I want to manage users within my own company (invite, edit, delete) so that I don't have to rely on the System Administrator for every personnel change.

## Acceptance Criteria
- [ ] **Role Definition:** Create a new role `CompanyAdmin`.
- [ ] **Permissions:**
    - `CompanyAdmin` can view the User Management list.
    - `CompanyAdmin` ONLY sees users belonging to their own company.
    - `CompanyAdmin` can Create/Edit/Delete users ONLY within their own company.
    - `CompanyAdmin` CANNOT modify users from other companies or System Admins.
    - `CompanyAdmin` CANNOT assign the `SystemAdmin` role to anyone.
- [ ] **UI:** Update `UserManagement` page to support this filtered view and restricted actions.

## Technical Notes
-   Update `UserRole` constants.
-   Update `UsersController` and `UserService` to enforce company-based scoping for CRUD operations.
-   Ensure `Permissions.Users.Manage` policy allows `CompanyAdmin` but the *implementation* restricts the scope.

## Dependencies
-   Story 3.6 (Company Identity) - Completed.
