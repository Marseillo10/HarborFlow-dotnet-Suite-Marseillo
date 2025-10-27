# HarborFlow - Smart Port Management System

HarborFlow is a modern application designed to digitize and streamline port operations. It features real-time vessel tracking, a digital service request workflow, and a maritime news aggregator. The project includes both a WPF desktop application (for Windows) and a Blazor web application (for cross-platform use).

For detailed technical information, system architecture, and feature breakdowns, please see the **[Full Project Documentation](md_Files/HarborFlow_Full_Documentation.md)**.

## Project Status (October 2025)

- **Build:** The entire solution builds cleanly on the .NET 9 SDK.
- **Backend:** The core backend logic, services, and API integrations are functionally complete and stable.
- **Frontend:**
    - **WPF App:** The original user interface, correctly structured but not the focus of recent development.
    - **Web App:** The Blazor web application is now **fully functional**, providing a modern, cross-platform interface for all core features of the project.

## Key Features

- **Live Map:** Real-time vessel tracking on an interactive map with multiple layer options (Street, Satellite, Nautical).
- **Advanced Maritime News Feed:**
    - Aggregates news from dozens of international, national, and official sources.
    - Intelligent keyword filtering to ensure maritime relevance.
    - Categorized views (International, National, Official, All Feeds).
    - Interactive date filters (24h, 7d, 30d, All Time) with auto-scrolling.
- **Service & Vessel Workflow:**
    - **Analytics Dashboard:** Visual charts for service request statuses and vessel types. This feature is currently available only for logged-in users.
    - **Vessel Management:** Full CRUD (Create, Read, Update, Delete) functionality for vessel data.
    - **Service Request Management:** This feature is currently under development for the web application.
- **User Experience:**
    - **Access Modes:** The application supports both guest and registered user modes. Core features like the live map and news feed are available to all. Full data management capabilities are restricted to logged-in users.
    - **Map Bookmarking:** This feature is currently under development. The goal is to allow users to save and manage map locations.

## Tech Stack

- **Frontend:** .NET 9 WPF, Blazor Server
- **Backend:** .NET 9 Class Libraries
- **Database:** PostgreSQL (managed via Docker)
- **UI Libraries:** ChartJs.Blazor, Leaflet.js
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

    - **Web Application (Recommended):**
        ```bash
        dotnet run --project HarborFlow.Web
        ```
        Then open `http://localhost:5275` in your browser.

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

> **Note for macOS/Linux users:** You can build, test, and contribute to all non-UI projects. The Blazor web application is the recommended way to run and interact with the application on non-Windows systems. See [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) for more details.
