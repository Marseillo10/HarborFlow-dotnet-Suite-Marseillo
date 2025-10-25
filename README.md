# HarborFlow - Smart Port Management System

HarborFlow is a modern desktop application designed to digitize and streamline port operations. Built with .NET 9 WPF, it features real-time vessel tracking, a digital service request workflow, and a maritime news aggregator.

For detailed technical information, system architecture, and feature breakdowns, please see the **[Full Project Documentation](md%20Files/HarborFlow_Full_Documentation.md)**.

## Key Features

- **Live Map:** Real-time vessel tracking on an interactive map.
- **Maritime News:** An integrated news feed with articles from leading maritime sources.
- **Service Workflow:** A complete system for submitting, approving, and managing port service requests (requires login).
- **Map Bookmarks:** Save and quickly navigate to important map areas (requires login).
- **Guest Mode:** Explore public features like the map and news feed without needing an account.

## Tech Stack

- **Frontend:** .NET 9 WPF
- **Backend:** .NET 9 Class Libraries
- **Database:** PostgreSQL (managed via Docker)
- **Architecture:** Clean Architecture, MVVM Pattern

## Quick Start (Recommended)

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
    dotnet ef database update --project HarborFlow.Infrastructure
    ```

4.  **Run the application (Windows only):**
    ```bash
    dotnet run --project HarborFlow.Wpf
    ```

> **Note for macOS/Linux users:** You can build and contribute to the non-UI projects (`Core`, `Application`, `Infrastructure`), but you cannot run the WPF user interface. See [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) for more details.