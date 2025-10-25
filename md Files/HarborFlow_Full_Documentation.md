# HarborFlow - Full Project Documentation

## 1. Project Overview

HarborFlow is a modern Smart Port Management System designed to digitize and streamline operational workflows. Built with .NET 9, this project uses a WPF desktop application for the user interface and follows Clean Architecture principles to ensure a scalable and maintainable codebase.

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
- **Model-View-ViewModel (MVVM) Pattern:** The WPF UI is built using the MVVM pattern, which separates the user interface (View) from the business logic and data (ViewModel).
- **Dependency Injection (DI):** The application uses the built-in .NET DI container to manage dependencies, promoting loose coupling and testability. All services are registered in `App.xaml.cs`.
- **Centralized Notification Hub:** A custom `INotificationHub` service acts as a central event bus for displaying user-facing messages, decoupling business logic from UI notifications.

### 2.2. Project Layers

- **`HarborFlow.Core` (Domain):** The heart of the application. Contains all business models (entities) and application-specific interfaces (contracts). It has zero dependencies on other layers.
- **`HarborFlow.Application` (Application):** Contains the application-specific business logic. It orchestrates the domain models to perform tasks and implements the interfaces defined in Core.
- **`HarborFlow.Infrastructure` (Infrastructure):** Handles all external concerns: database access (Entity Framework), communication with third-party APIs (AIS data, RSS feeds), etc.
- **`HarborFlow.Wpf` (Presentation):** The user interface layer, built with WPF and the MVVM pattern.

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
    dotnet ef database update --project HarborFlow.Infrastructure
    ```

4.  **Run the Application (Windows Only):**
    ```bash
    dotnet run --project HarborFlow.Wpf
    ```

### 3.3. Platform-Specific Development

While the UI is Windows-only, the core logic is cross-platform. Developers on **macOS or Linux** can build, test, and contribute to the `HarborFlow.Core`, `HarborFlow.Application`, and `HarborFlow.Infrastructure` projects. For more details, see the [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) file.

## 4. Implementation Details

This section covers the technical "how-to" of the codebase, including configuration, specific WPF patterns, and external service integrations.

### 4.1. Configuration Management (`appsettings.json`)

The application uses a central `appsettings.json` file to manage external configurations. This allows settings to be changed without modifying the source code.

-   **`ConnectionStrings`**: Contains the `DefaultConnection` string used by Entity Framework Core to connect to the PostgreSQL database.
-   **`ApiKeys`**: Contains the `AisStream` key required for the real-time vessel tracking service.
-   **`RssFeeds`**: A list of URLs used by the `RssService` to aggregate articles for the News feature.

### 4.2. WPF Presentation Layer (MVVM)

To support the MVVM pattern effectively, the `HarborFlow.Wpf` project includes several key helper components:

-   **Commands (`RelayCommand` & `AsyncRelayCommand`)**: These classes bridge the UI (e.g., a button click) and the logic in the `ViewModel`. `RelayCommand` is for synchronous actions, while `AsyncRelayCommand` handles asynchronous operations and prevents duplicate executions.
-   **Custom UI Services**: Services like `IWindowManager` (to manage windows and dialogs), `ISettingsService` (to handle user themes), and `IFileService` (for file operations) are used to abstract UI-specific logic out of the ViewModels, making them more testable.
-   **Value Converters**: Small helper classes like `BooleanToVisibilityConverter` are used in XAML to translate data (e.g., a `bool`) into a UI property (e.g., `Visibility.Visible`), keeping presentation logic out of the ViewModel.

### 4.3. External API Integration (AIS Data)

A core feature of HarborFlow is its ability to track vessels in real-time via the `AisStreamService`.

-   **Provider:** AISstream (`stream.aisstream.io`)
-   **Implementation:** The service establishes a persistent WebSocket connection to the AISstream server, subscribes to a global bounding box, and listens for incoming position reports. It raises a `PositionReceived` event that other parts of the application use to update the map.
-   **Note:** The `AisDataService` currently returns placeholder data, as the previous non-free data source has been removed.

### 4.4. Key Third-Party Libraries

The project leverages several key open-source libraries:

-   **`BCrypt.Net-Next`**: Used for securely hashing user passwords.
-   **`FluentValidation`**: Provides a clear way to build validation rules for business objects.
-   **`LiveCharts.Wpf`**: The core library used for creating the interactive charts in the Analytics Dashboard.
-   **`Microsoft.Web.WebView2`**: Enables the WPF application to host web content, which is crucial for rendering the Leaflet.js interactive map.
-   **Testing Stack (`xunit`, `Moq`, `FluentAssertions`)**: The project relies on a standard, robust stack for unit and integration testing.

## 5. Feature Breakdown

### 5.1. Guest Mode & Authentication

- The application starts in a "Guest" state, allowing exploration of public features.
- UI elements are dynamically shown or hidden based on the user's login status (`IsLoggedIn` property).
- The `SessionContext` service manages the current user's state and fires a `UserChanged` event on login/logout to refresh the UI.

### 5.2. Analytics Dashboard

- The dashboard uses the `LiveCharts.Wpf` library to display a pie chart of service requests by status and a column chart of vessels by type.

### 5.3. Map Bookmarks

- Logged-in users can save, delete, and navigate to geographic bookmarks. The bookmark controls are only visible when a user is authenticated.

### 5.4. Maritime News Feed

- The `RssService` fetches articles from a list of URLs in `appsettings.json`.
- The `NewsViewModel` filters articles first by maritime keywords and then by user search input.

### 5.5. Interactive Map & Data Layers

- The map is powered by **Leaflet.js** and rendered in a `WebView2` control in WPF.
- It features switchable tile layers: **OpenStreetMap** (default), **Esri World Imagery** (satellite), and **OpenSeaMap** (nautical).

## 6. Future Development Roadmap

- **Phase 2: Advanced Map Features**
    - **Geofencing:** Allow users to draw zones on the map and receive alerts.
    - **Vessel History Playback:** Animate the historical track of a selected vessel.
- **Phase 3: Architectural Evolution**
    - **Decouple to Web API:** Refactor the business logic into a separate ASP.NET Core Web API project.
- **Phase 4: Enhanced Collaboration**
    - **Shared Bookmarks:** Allow users within the same organization to share map bookmarks.
    - **Real-time Notifications with SignalR:** Replace the current event-based system with SignalR.
