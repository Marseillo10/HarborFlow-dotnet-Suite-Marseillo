# HarborFlow - Smart Port Management System

HarborFlow is a modern application designed to digitize and streamline port operations. It features real-time vessel tracking, a digital service request workflow, and a maritime news aggregator. The project includes both a WPF desktop application (for Windows) and a Blazor web application (for cross-platform use).

For detailed technical information, system architecture, and feature breakdowns, please see the **[Full Project Documentation](md_Files/HarborFlow_Full_Documentation.md)**.

## Project Status (October 2025)

- **Build:** The entire solution builds cleanly on the .NET 9 SDK.
- **Backend:** The core backend logic (services, data access, API integration) has undergone a comprehensive functional analysis. All key services have been verified with automated tests and are considered functionally stable.
- **Frontend:**
    - **WPF App:** The WPF user interface code has been analyzed and is correctly structured, but has not been visually or manually tested.
    - **Web App:** A Blazor web application has been added to provide cross-platform access to the core features. It is partially functional and under active development.

## Key Features

- **Live Map:** Real-time vessel tracking on an interactive map.
- **Maritime News:** An integrated news feed with articles from leading maritime sources.
- **Service Workflow:** A complete system for submitting, approving, and managing port service requests (requires login).
- **Map Bookmarks:** Save and quickly navigate to important map areas (requires login).
- **Guest Mode:** Explore public features like the map and news feed without needing an account.

## Tech Stack

- **Frontend:** .NET 9 WPF, Blazor
- **Backend:** .NET 9 Class Libraries
- **Database:** PostgreSQL (managed via Docker)
- **Testing:** xUnit, Moq, FluentAssertions
- **Architecture:** Clean Architecture, MVVM Pattern (WPF), MVU Pattern (Blazor)

## Quick Start

This project uses Docker to provide a consistent and easy-to-use database environment.

### Prerequisites

- .NET 9 SDK
- Docker Desktop (must be running)

### Steps

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Marseillo10/HarborFlow_WPF.git
    cd HarborFlow_WPF
    ```

2.  **Start the database:**
    ```bash
    docker-compose up -d
    ```

3.  **Apply database migrations (first time only):**
    ```bash
    dotnet ef database update --startup-project HarborFlow.Web --project HarborFlow.Infrastructure
    ```

4.  **Run the application:**

    - **Web Application (Cross-platform):**
        ```bash
        dotnet run --project HarborFlow.Web
        ```

    - **WPF Application (Windows only):**
        ```bash
        dotnet run --project HarborFlow.Wpf
        ```

## Development & Testing

This solution contains two test projects:

- `HarborFlow.Tests`: Contains tests for WPF-dependent components, such as ViewModels. **Requires Windows to run.**
- `HarborFlow.Backend.Tests`: A cross-platform project for testing the core backend logic (`Core`, `Application`, `Infrastructure`). **Can be run on macOS, Linux, or Windows.**

### How to Run Tests

To run the cross-platform backend tests from the root directory:

```bash
dotnet test HarborFlow.Backend.Tests/
```

> **Note for macOS/Linux users:** You can build, test, and contribute to all non-UI projects. The Blazor web application is the recommended way to run the application on non-Windows systems. See [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) for more details.