# HarborFlow Suite - BMAD Documentation

This document provides an overview of the HarborFlow Suite project for the purpose of applying the BMAD (Bionic-Model-Assisted Development) methodology.

## 1. Project Overview

HarborFlow Suite is a comprehensive software solution for the maritime industry. It appears to be a web application built with .NET.

*(TODO: Marcel, please add a more detailed description of the project's purpose and goals.)*

## 2. Existing Technologies

Based on the project structure, the following technologies are used:

*   **Backend:** C# with .NET
*   **Frontend:** Blazor WebAssembly
*   **Database:** PostgreSQL. 
    > **Note:** The connection string was provided and is configured for local development. Ensure that secrets and connection strings for production are managed securely and not committed to source control.

## 3. Architectural Overview

The project follows a Clean Architecture pattern, separating concerns into the following layers:

*   `HarborFlowSuite.Core`: Contains core domain models and business logic.
*   `HarborFlowSuite.Application`: Contains application-specific logic and services.
*   `HarborFlowSuite.Infrastructure`: Handles external concerns like database access, file systems, etc.
*   `HarborFlowSuite.Server`: The main server project, likely hosting the Blazor application and API endpoints.
*   `HarborFlowSuite.Client`: The Blazor WebAssembly client-side application.

## 4. How to Run the Project

The project can be run using the following commands from the root directory:
```
# Run the server
dotnet run --project HarborFlowSuite/HarborFlowSuite.Server --launch-profile https

# Run the client
dotnet run --project HarborFlowSuite/HarborFlowSuite.Client --launch-profile https
```
The application will be available at `https://localhost:7163/` or `http://localhost:5205`.

## 5. How to Run Tests

The project includes test projects:
* `HarborFlowSuite.Application.Tests`
* `HarborFlowSuite.Server.Tests`

To run tests in this .NET project, you can use the dotnet test command.

  To run all tests in the solution:
   1 dotnet test HarborFlowSuite/HarborFlowSuite.sln

  To run tests for a specific project, navigate to the project directory or specify the project file:
   1 dotnet test HarborFlowSuite/HarborFlowSuite.Application.Tests/HarborFlowSuite.Application.Tests.csproj
  or

   1 dotnet test HarborFlowSuite/HarborFlowSuite.Server.Tests/HarborFlowSuite.Server.Tests.csproj