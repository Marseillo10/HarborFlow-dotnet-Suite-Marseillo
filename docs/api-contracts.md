# API Contracts

This document provides an overview of the API endpoints exposed by the `HarborFlowSuite.Server` application.

## Authentication

-   **POST /api/auth/register**: Register a new user.
    -   **Request Body**: `RegisterUserDto`
    -   **Response Body**: `User`
    -   **Authentication**: Not required.

## Company

-   **GET /api/company**: Get all companies.
    -   **Response Body**: `IEnumerable<Company>`
    -   **Authentication**: Required.
-   **GET /api/company/{id}**: Get a company by ID.
    -   **Response Body**: `Company`
    -   **Authentication**: Required.
-   **POST /api/company**: Create a new company.
    -   **Request Body**: `CreateCompanyDto`
    -   **Response Body**: `Company`
    -   **Authentication**: Required.
-   **PUT /api/company/{id}**: Update a company.
    -   **Request Body**: `Company`
    -   **Authentication**: Required.
-   **DELETE /api/company/{id}**: Delete a company.
    -   **Authentication**: Required.

## Dashboard

-   **GET /api/dashboard/servicerequeststatus**: Get a summary of service requests by status.
    -   **Response Body**: `IEnumerable<ServiceRequestStatusSummaryDto>`
    -   **Authentication**: Required.
-   **GET /api/dashboard/vesseltypes**: Get a summary of vessels by type.
    -   **Response Body**: `IEnumerable<VesselTypeSummaryDto>`
    -   **Authentication**: Required.

## Map Bookmark

-   **GET /api/mapbookmark**: Get all map bookmarks for the current user.
    -   **Response Body**: `IEnumerable<MapBookmark>`
    -   **Authentication**: Required.
-   **GET /api/mapbookmark/{id}**: Get a map bookmark by ID.
    -   **Response Body**: `MapBookmark`
    -   **Authentication**: Required.
-   **POST /api/mapbookmark**: Create a new map bookmark.
    -   **Request Body**: `CreateMapBookmarkDto`
    -   **Response Body**: `MapBookmark`
    -   **Authentication**: Required.
-   **PUT /api/mapbookmark/{id}**: Update a map bookmark.
    -   **Request Body**: `MapBookmark`
    -   **Authentication**: Required.
-   **DELETE /api/mapbookmark/{id}**: Delete a map bookmark.
    -   **Authentication**: Required.

## Service Request

-   **GET /api/servicerequest**: Get all service requests.
    -   **Response Body**: `List<ServiceRequest>`
    -   **Authentication**: Required.
-   **POST /api/servicerequest**: Create a new service request.
    -   **Request Body**: `ServiceRequest`
    -   **Response Body**: `ServiceRequest`
    -   **Authentication**: Required.
-   **POST /api/servicerequest/{id}/approve**: Approve a service request.
    -   **Request Body**: `ApprovalDto`
    -   **Authentication**: Required.
-   **POST /api/servicerequest/{id}/reject**: Reject a service request.
    -   **Request Body**: `ApprovalDto`
    -   **Authentication**: Required.

## User Profile

-   **GET /api/userprofile**: Get the user profile of the current user.
    -   **Response Body**: `UserProfileDto`
    -   **Authentication**: Required.
-   **PUT /api/userprofile**: Update the user profile of the current user.
    -   **Request Body**: `UserProfileDto`
    -   **Authentication**: Required.

## Vessel

-   **GET /api/vessel**: Get all vessels.
    -   **Response Body**: `List<Vessel>`
    -   **Authentication**: Not required.

## Weather Forecast

-   **GET /weatherforecast**: Get a weather forecast.
    -   **Response Body**: `IEnumerable<WeatherForecast>`
    -   **Authentication**: Required.