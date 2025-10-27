# HarborFlow - Full Project Documentation

## 1. Project Overview

HarborFlow is a modern Smart Port Management System designed to digitize and streamline operational workflows. Built with .NET 9, this project includes a WPF desktop application for Windows and a Blazor web application for cross-platform accessibility. It follows Clean Architecture principles to ensure a scalable and maintainable codebase.

### 1.1. Core Mission

To provide a centralized, intuitive, and efficient digital platform for all port stakeholders, reducing manual processes, improving data visibility, and enhancing operational coordination.

### 1.2. Key Features

- **Guest & Registered User Modes:** Users can immediately explore public features, with more advanced capabilities available after login.
- **Interactive Real-time Map:** Track vessel positions using live AIS data.
- **Map Bookmarks:** Logged-in users can save and quickly navigate to important custom map locations.
- **Maritime News Feed:** An integrated news aggregator with keyword filtering, pulling from curated maritime RSS feeds.
- **Digital Service Workflow:** A complete system for submitting, approving, and managing port service requests for registered users.
- **Analytics Dashboard:** Visual charts displaying key port metrics like vessel distribution and service request statuses.

## 2. System Architecture

HarborFlow is built on a foundation of modern software design principles to ensure it is testable, scalable, and easy to maintain.

### 2.1. Architectural Principles

- **Clean Architecture:** The project follows this principle to create a separation of concerns. Dependencies flow inwards: UI and Infrastructure depend on the Application and Core layers, not the other way around.
- **UI Patterns:**
    - **Model-View-ViewModel (MVVM) Pattern:** The WPF UI is built using the MVVM pattern, which separates the user interface (View) from the business logic and data (ViewModel).
    - **Model-View-Update (MVU) Pattern:** The Blazor web application follows the MVU pattern, where the UI is a function of the application state.
- **Dependency Injection (DI):** The application uses the built-in .NET DI container to manage dependencies, promoting loose coupling and testability.
- **Centralized Notification Hub:** A custom `INotificationHub` service acts as a central event bus for displaying user-facing messages, decoupling business logic from UI notifications.

### 2.2. Project Layers

- **`HarborFlow.Core` (Domain):** The heart of the application. Contains all business models (entities) and application-specific interfaces (contracts). It has zero dependencies on other layers.
- **`HarborFlow.Application` (Application):** Contains the application-specific business logic. It orchestrates the domain models to perform tasks and implements the interfaces defined in Core.
- **`HarborFlow.Infrastructure` (Infrastructure):** Handles all external concerns: database access (Entity Framework), communication with third-party APIs (AIS data, RSS feeds), etc.
- **`HarborFlow.Wpf` (Presentation):** The user interface layer for Windows, built with WPF and the MVVM pattern.
- **`HarborFlow.Web` (Presentation):** The user interface layer for cross-platform access, built with Blazor.

### 2.3. Core Data Models

The domain layer is built around a set of core entities that represent the key concepts in the port management system.

-   **`User`**: Represents an individual who can log in to the system.
    -   **Key Properties**: `UserId`, `Username`, `PasswordHash`, `Email`, `Role`.
    -   **Description**: Stores user credentials, personal information, and their assigned role (e.g., Administrator, PortOfficer), which dictates their permissions.

-   **`Vessel`**: Represents a single maritime vessel.
    -   **Key Properties**: `IMO` (Primary Key), `Mmsi`, `Name`, `VesselType`.
    -   **Description**: Contains static details about a vessel, such as its unique identifiers, name, and type.

-   **`VesselPosition`**: Represents a single, time-stamped geographical position of a vessel.
    -   **Key Properties**: `VesselImo` (Foreign Key), `Latitude`, `Longitude`, `PositionTimestamp`.
    -   **Description**: Stores the historical and real-time track of a vessel.

-   **`ServiceRequest`**: Represents a request for a port service.
    -   **Key Properties**: `RequestId`, `VesselImo`, `RequestedBy` (UserId), `ServiceType`, `Status`.
    -   **Description**: Tracks the service type, target vessel, requester, and status in the approval workflow.

-   **`MapBookmark`**: Represents a user-saved geographical area on the map.
    -   **Key Properties**: `Name`, `UserId`, `North`, `South`, `East`, `West`.
    -   **Description**: Allows authenticated users to save and quickly return to specific map regions.

## 3. Development & Setup

This project is configured to use Docker for the PostgreSQL database for a consistent development environment.

### 3.1. Prerequisites

- **.NET 9 SDK**
- **Docker Desktop:** Must be installed and running.
- **IDE:** Visual Studio 2022 (recommended for Windows) or any compatible editor like VS Code.

### 3.2. Quick Start Guide

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Marseillo10/HarborFlow_WPF.git
    cd HarborFlow_WPF
    ```

2.  **Start the Database Container:**
    ```bash
    docker-compose up -d
    ```

3.  **Apply Database Migrations (First Time Only):**
    ```bash
    dotnet ef database update --startup-project HarborFlow.Web --project HarborFlow.Infrastructure
    ```

4.  **Run the Application:**

    - **Web Application (Cross-platform):**
        ```bash
        dotnet run --project HarborFlow.Web
        ```

    - **WPF Application (Windows only):**
        ```bash
        dotnet run --project HarborFlow.Wpf
        ```

### 3.3. Platform-Specific Development

While the WPF UI is Windows-only, the web application and the core logic are cross-platform. Developers on **macOS or Linux** can build, test, and contribute to all projects except `HarborFlow.Wpf`. For more details, see the [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) file.

## 4. Implementation Details

This section covers the technical \"how-to\" of the codebase, including configuration, specific UI patterns, and external service integrations.

### 4.1. Configuration Management (`appsettings.json`)

The application uses a central `appsettings.json` file to manage external configurations. This allows settings to be changed without modifying the source code.

-   **`ConnectionStrings`**: Contains the `DefaultConnection` string for the PostgreSQL database.
-   **`ApiKeys`**:\
    -   `AisStream`: The key for the real-time position streaming service.
    -   `GlobalFishingWatch`: The key for the vessel detail and identity service.
-   **`RssFeeds`**: A list of URLs for the maritime news aggregator.

### 4.2. Presentation Layers

#### 4.2.1. WPF (MVVM)

To support the MVVM pattern effectively, the `HarborFlow.Wpf` project includes several key helper components:

-   **Commands (`RelayCommand` & `AsyncRelayCommand`)**: These classes bridge the UI (e.g., a button click) and the logic in the `ViewModel`. `RelayCommand` is for synchronous actions, while `AsyncRelayCommand` handles asynchronous operations and prevents duplicate executions.
-   **Custom UI Services**: Services like `IWindowManager` (to manage windows and dialogs), `ISettingsService` (to handle user themes), and `IFileService` (for file operations) are used to abstract UI-specific logic out of the ViewModels, making them more testable.
-   **Value Converters**: Small helper classes like `BooleanToVisibilityConverter` are used in XAML to translate data (e.g., a `bool`) into a UI property (e.g., `Visibility.Visible`), keeping presentation logic out of the ViewModel.

#### 4.2.2. Blazor (MVU)

The `HarborFlow.Web` project uses Blazor to create a cross-platform web UI.

-   **Components:** The UI is built as a series of `.razor` components, which combine HTML markup and C# code.
-   **Dependency Injection:** Services are injected into components using the `@inject` directive.
-   **JavaScript Interop:** The application uses JavaScript interop to interact with JavaScript libraries like Leaflet.js for the interactive map.

### 4.3. External API Integration (AIS Data)

The application uses a dual-service approach to handle vessel data: one for real-time positions and another for detailed vessel identity information.

-   **`AisStreamService` (Real-time Positions):**
    -   **Provider:** AISstream (`stream.aisstream.io`)
    -   **Implementation:** This service establishes a persistent WebSocket connection to receive live position updates for vessels, which are then displayed on the map.

-   **`AisDataService` (Vessel Details):**
    -   **Provider:** Global Fishing Watch (`api.globalfishingwatch.org`)
    -   **Implementation:** This service replaces the previous placeholder data. When details for a specific vessel are needed, it makes a REST API call to the Global Fishing Watch `v2/vessels` endpoint, authenticated with a Bearer Token. It searches for the vessel by its IMO number and maps the resulting data (name, type, flag, etc.) to the application\'s `Vessel` model.
    -   **Note:** This integration is free for non-commercial use, which aligns with this project\'s academic purpose.

### 4.4. Key Third-Party Libraries

The project leverages several key open-source libraries:

-   **`BCrypt.Net-Next`**: Used for securely hashing user passwords.
-   **`FluentValidation`**: Provides a clear way to build validation rules for business objects.
-   **`LiveCharts.Wpf`**: The core library used for creating the interactive charts in the Analytics Dashboard (WPF only).
-   **`Microsoft.Web.WebView2`**: Enables the WPF application to host web content, which is crucial for rendering the Leaflet.js interactive map.
-   **`Leaflet.js`**: Used in the Blazor web application to render the interactive map.
-   **Testing Stack (`xunit`, `Moq`, `FluentAssertions`)**: The project relies on a standard, robust stack for unit and integration testing.

## 5. Feature Breakdown

### 5.1. Web Application - Current Status (October 2025)

The Blazor web application is the primary focus of development. Here is the current status of its features:

- **Real-Time Map (`MapView`):** Fully functional. Displays real-time vessel positions via WebSocket and includes interactive map layer controls.
- **Advanced News Feed (`NewsView`):** Fully functional. Features categorization, intelligent keyword filtering, and interactive date filters.
- **Analytics Dashboard (`DashboardView`):** Implemented for logged-in users. Guest access is not currently available.
- **Map Bookmarking (`MapView`):** This feature is currently under development. The goal is to allow users to save and manage important map locations.
- **Service Request Management (`ServiceRequestView`):** The web interface for this feature is currently under development.

### 5.2. Guest Mode & Authentication

- The application supports both guest and registered user modes.
- Core informational features (Live Map, News Feed) are available to guests.
- Data management features (Vessel Management, Service Requests, Dashboard) and account-specific features (Bookmarks) are restricted to authenticated users.
- The `SessionContext` service manages the user's state and UI updates on login/logout.

### 5.3. Platform-Specific Implementations

- **WPF Dashboard:** Uses the `LiveCharts.Wpf` library.
- **Map Rendering:**
    - **WPF:** Uses **Leaflet.js** rendered in a `WebView2` control.
    - **Web:** Uses **Leaflet.js** rendered directly in a Blazor component.
- **Map Layers:** Both platforms support **OpenStreetMap**, **Esri World Imagery**, and **OpenSeaMap**.

## 6. Future Development Roadmap

- **Phase 2: Advanced Map Features**
    - **Geofencing:** Allow users to draw zones on the map and receive alerts.
    - **Vessel History Playback:** Animate the historical track of a selected vessel.
- **Phase 3: Architectural Evolution**
    - **Decouple to Web API:** Refactor the business logic into a separate ASP.NET Core Web API project.
- **Phase 4: Enhanced Collaboration**
    - **Shared Bookmarks:** Allow users within the same organization to share map bookmarks.
    - **Real-time Notifications with SignalR:** Replace the current event-based system with SignalR.
