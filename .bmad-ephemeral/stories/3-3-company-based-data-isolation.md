# Story 3.3: Company-Based Data Isolation

Status: completedcompleted

## Description
As a **System Administrator**, I want to ensure that users can only access data belonging to their own company, so that sensitive information is isolated between different organizations using the platform.

## Acceptance Criteria
- [x] **Data Filtering:** API endpoints returning lists of data (e.g., Vessels, Service Requests) must automatically filter results based on the authenticated user's `CompanyId`.
- [x] **Access Control:** Users attempting to access a specific resource (e.g., `GET /api/vessels/{id}`) that belongs to another company must receive a `403 Forbidden` or `404 Not Found` response.
- [x] **Creation Assignment:** When a user creates a new resource (e.g., a Service Request), it must automatically be assigned to the user's `CompanyId`.
- [x] **Admin Override:** Users with the `System Administrator` role (or a specific `Data.ViewAll` permission) should be able to access data across all companies.

## Technical Notes
-   **Global Query Filters:** Consider using EF Core Global Query Filters to automatically apply the `CompanyId` restriction to all queries for tenant-specific entities.
-   **CurrentCompanyService:** Implement a service to retrieve the current user's `CompanyId` from the HTTP context/claims.
-   **Entities Affected:** `Vessel`, `ServiceRequest`, `User` (for viewing other users), `Port` (maybe shared?), `Company` (users can only see their own).

## Dependencies
-   Story 3.1 (Roles) and 3.2 (Permissions) must be complete to identify the user and their role.
