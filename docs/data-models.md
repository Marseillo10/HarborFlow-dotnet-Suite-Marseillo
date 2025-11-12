# Data Models

This document provides an overview of the data models used in the HarborFlow Suite application.

## Core Entities

-   **User**: Represents a user of the application.
    -   **Relationships**: Belongs to one `Company` and has one `Role`. Has many `ServiceRequests`, `MapBookmarks`, and `ApprovalHistories`.
-   **Company**: Represents a company that uses the application.
    -   **Relationships**: Has many `Users` and `Vessels`.
-   **Vessel**: Represents a vessel.
    -   **Relationships**: Belongs to one `Company`. Has many `VesselPositions`.
-   **ServiceRequest**: Represents a service request.
    -   **Relationships**: Belongs to one `User` (requester) and one `Vessel`. Has many `ApprovalHistories`.
-   **MapBookmark**: Represents a map bookmark.
    -   **Relationships**: Belongs to one `User`.
-   **ApprovalHistory**: Represents the approval history of a service request.
    -   **Relationships**: Belongs to one `ServiceRequest` and one `User` (approver).
-   **Role**: Represents a user role.
    -   **Relationships**: Has many `Users` and `RolePermissions`.
-   **Permission**: Represents a permission.
    -   **Relationships**: Has many `RolePermissions`.
-   **RolePermission**: Represents the relationship between a role and a permission.
    -   **Relationships**: Belongs to one `Role` and one `Permission`.
-   **VesselPosition**: Represents the position of a vessel at a specific time.
    -   **Relationships**: Belongs to one `Vessel`.

## Enums

-   **Roles**: Defines the user roles (`Guest`, `VesselAgent`, `PortAuthorityOfficer`, `SystemAdministrator`).
-   **ServiceRequestStatus**: Defines the status of a service request (`Pending`, `Approved`, `Rejected`, `Cancelled`, `Completed`, `InProgress`).

## Data Transfer Objects (DTOs)

-   **CreateCompanyDto**: DTO for creating a new company.
    -   `Name` (string, required)
-   **CreateMapBookmarkDto**: DTO for creating a new map bookmark.
    -   `Name` (string, required)
    -   `Latitude` (double, required)
    -   `Longitude` (double, required)
-   **CreateServiceRequestDto**: DTO for creating a new service request.
    -   `Title` (string, required)
    -   `Description` (string, required)
    -   `Status` (string, required)
-   **CreateVesselDto**: DTO for creating a new vessel.
    -   `Name` (string, required)
    -   `IMO` (string)
    -   `Type` (string, required)
-   **LoginUserDto**: DTO for user login.
    -   `Email` (string, required)
    -   `Password` (string, required)
-   **RegisterUserDto**: DTO for user registration.
    -   `Email` (string, required)
    -   `Password` (string, required)
    -   `Name` (string)
-   **ServiceRequestStatusSummaryDto**: DTO for the service request status summary.
    -   `Status` (string)
    -   `Count` (int)
-   **UserProfileDto**: DTO for the user profile.
    -   `FullName` (string)
    -   `Email` (string)
-   **VesselTypeSummaryDto**: DTO for the vessel type summary.
    -   `VesselType` (string)
    -   `Count` (int)