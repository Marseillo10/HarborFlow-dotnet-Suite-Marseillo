# Source Tree Analysis

This document provides an analysis of the source code directory structure, highlighting critical directories, entry points, and key file locations.

## Project Structure

The project is a multi-part application with the following components:

-   **HarborFlowSuite.Client**: Blazor WebAssembly (web)
-   **HarborFlowSuite.Server**: ASP.NET Core Web API (backend)
-   **HarborFlowSuite.Application**: .NET Library
-   **HarborFlowSuite.Core**: .NET Library
-   **HarborFlowSuite.Infrastructure**: .NET Library

## Annotated Directory Tree

### HarborFlowSuite.Client (web)

-   **/Pages**: Contains the Blazor pages (routable components).
-   **/Shared**: Contains shared Blazor components.
-   **/wwwroot**: Contains static assets, including `index.html` and `service-worker.js`.
-   **Program.cs**: The entry point of the Blazor application.

### HarborFlowSuite.Server (backend)

-   **/Controllers**: Contains the API controllers that handle incoming HTTP requests.
-   **/Hubs**: Contains the SignalR hubs for real-time communication.
-   **Program.cs**: The entry point of the ASP.NET Core application.
-   **appsettings.json**: Configuration file for the application.

### HarborFlowSuite.Application (library)

-   **/Services**: Contains the interfaces for the application services.

### HarborFlowSuite.Core (library)

-   **/Models**: Contains the domain models (entities).
-   **/DTOs**: Contains the Data Transfer Objects.

### HarborFlowSuite.Infrastructure (library)

-   **/Persistence**: Contains the database context and migrations.
-   **/Services**: Contains the implementation of the application services.
