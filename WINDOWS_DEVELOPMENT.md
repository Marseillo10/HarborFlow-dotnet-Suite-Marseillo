# Windows-Specific & Cross-Platform Development Guide

This document outlines development tasks and limitations for both Windows and non-Windows (macOS, Linux) environments.

## Background

While the majority of the codebase (.NET Class Libraries for `Core`, `Application`, and `Infrastructure`) is cross-platform, the user interface (`HarborFlow.Wpf`) and the main test project (`HarborFlow.Tests`) are Windows-dependent. This guide clarifies what can and cannot be done on each platform.

## Development Workflow on macOS/Linux

Developers on non-Windows systems can fully contribute to the core backend logic of the application. Here is the recommended workflow.

### 1. Initial Setup

Ensure you have the following installed:
- **.NET 9 SDK**: The core software development kit.
- **Docker Desktop**: To run the PostgreSQL database container.

### 2. Database Setup

The database environment is managed by Docker, making it consistent across all platforms.

- **Start the database container:**
  ```shell
  docker-compose up -d
  ```
- **Apply database migrations (first time only):**
  ```shell
  dotnet ef database update --project HarborFlow.Infrastructure
  ```

### 3. Core Development Cycle

Your day-to-day development will focus on the backend projects:

- **Write & Modify Code**: You can freely create and edit features in `HarborFlow.Core`, `HarborFlow.Application`, and `HarborFlow.Infrastructure`.
- **Build the Solution**: After making changes, always run a build to check for compilation errors in the backend projects.
  ```shell
  dotnet build HarborFlow.sln
  ```

### 4. Important Limitations on macOS/Linux

- **You CANNOT run the WPF application**: The UI cannot be launched or debugged visually.
- **You CANNOT run the `HarborFlow.Tests` project**: The main test project is Windows-only because it references the WPF framework (`Microsoft.WindowsDesktop.App.WPF`). This is necessary to test UI-related components like ViewModels but prevents the tests from running on macOS or Linux.

## Windows-Only Tasks

Any work that requires visual validation or running the existing test suite must be done on a Windows machine.

### 1. Running and Debugging the UI

This is the only way to visually verify that the UI functions as expected.

- **Steps**: Open `HarborFlow.sln` in Visual Studio, set `HarborFlow.Wpf` as the startup project, and run.

### 2. Running the Test Suite

- **Steps**: Open a terminal in the project root and run `dotnet test`.

### 3. Verifying API Integrations

Testing integrations that require UI interaction must be done on Windows.

1.  **Jalankan Aplikasi**: `dotnet run --project HarborFlow.Wpf`
2.  **Amati Konsol**: Perhatikan log aplikasi di terminal.
3.  **Lakukan Aksi di UI**: Lakukan aksi yang memicu panggilan API (misalnya, mencari detail kapal).
4.  **Periksa Log**: Lihat output di konsol untuk pesan sukses atau error dari API.

## Recommendation for Improved Cross-Platform Testing

To enable backend testing on macOS/Linux, it is highly recommended to **create a new, separate test project** (e.g., `HarborFlow.Backend.Tests`). This new project would target `net9.0` (not `net9.0-windows`) and would only reference the `Core`, `Application`, and `Infrastructure` projects. 

This would allow developers on any platform to write and run unit tests for all backend logic, significantly improving the cross-platform development workflow.

## Conclusion

Core backend development is fully cross-platform. However, any work that touches the UI (`HarborFlow.Wpf`) or requires validation through the main test suite (`HarborFlow.Tests`) **must** ultimately be validated on a Windows environment.