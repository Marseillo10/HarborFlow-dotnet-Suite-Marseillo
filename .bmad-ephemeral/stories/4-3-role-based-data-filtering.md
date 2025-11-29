# Story 4.3: Role-Based Data Filtering

Status: completed

## Description
As a **System Admin**, I want to ensure that **Port Authority** users only see data (Vessels, Service Requests) relevant to their assigned Company, so that data privacy and isolation are maintained.

## Acceptance Criteria
- [x] **Global Filtering:** All queries for `Vessel` and `ServiceRequest` entities must automatically filter by `CompanyId` for non-admin users.
- [x] **Dashboard:** Dashboard charts and counters must reflect this filtered data.
- [x] **Management Pages:** Vessel and Service Request management pages must only show relevant records.

## Implementation Details
-   Implemented using Entity Framework Core **Global Query Filters** in `ApplicationDbContext`.
-   Filters check `_currentUserService.CompanyId` and `_currentUserService.IsSystemAdmin`.
-   Applied to `User`, `Vessel`, `ServiceRequest`, `ApprovalHistory`, and `VesselPosition` entities.
