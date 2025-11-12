# Architecture: HarborFlowSuite.Server

## 1. Executive Summary

This document outlines the architecture of the `HarborFlowSuite.Server` component, an ASP.NET Core Web API that serves as the backend for the HarborFlow Suite.

## 2. Technology Stack

-   **Framework**: ASP.NET Core
-   **Language**: C#
-   **Version**: .NET 9.0
-   **Dependencies**:
    -   Microsoft.AspNetCore.OpenApi
    -   Npgsql.EntityFrameworkCore.PostgreSQL
    -   FirebaseAdmin
    -   Microsoft.AspNetCore.Authentication.JwtBearer

## 3. Architecture Pattern

The server follows the Clean Architecture pattern, with a clear separation of concerns between the API controllers, application services, domain models, and infrastructure.

## 4. Data Architecture

The server uses a PostgreSQL database for data persistence, with Entity Framework Core as the ORM.

## 5. API Design

The server exposes a RESTful API with endpoints for all the application's functionalities. It also uses SignalR for real-time communication with the client.

## 6. Component Overview

-   **/Controllers**: Handle incoming HTTP requests and route them to the appropriate application services.
-   **/Hubs**: SignalR hubs for real-time communication.
-   **Program.cs**: The entry point of the application, where services are configured and the application is built.

## 7. Development Workflow

Refer to the main `development-guide.md` for instructions on how to set up and run the server application.

## 8. Deployment Architecture

The server can be deployed to any hosting provider that supports ASP.NET Core. Refer to the main `deployment-guide.md` for more details.

## 9. Testing Strategy

Unit and integration tests for the server are located in the `HarborFlowSuite.Server.Tests` project.
