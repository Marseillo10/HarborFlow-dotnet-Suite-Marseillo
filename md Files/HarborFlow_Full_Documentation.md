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

## 2. Development Environment & Setup

This project is configured to use Docker for the PostgreSQL database, which is the recommended approach for ensuring a consistent development environment across the team.

### 2.1. Prerequisites

- **.NET 9 SDK**
- **Docker Desktop:** Must be installed and running.
- **IDE:** Visual Studio 2022 (recommended for Windows) or any compatible editor like VS Code.

### 2.2. Quick Start Guide

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/Marseillo10/HarborFlow_WPF.git
    cd HarborFlow_WPF
    ```

2.  **Start the Database Container:**
    This command reads the `docker-compose.yml` file and starts a PostgreSQL database in a Docker container.
    ```bash
    docker-compose up -d
    ```

3.  **Apply Database Migrations (First Time Only):**
    This command creates the database schema (tables, columns, etc.) in the new Docker database.
    ```bash
    dotnet ef database update --project HarborFlow.Infrastructure
    ```

4.  **Run the Application (Windows Only):**
    The WPF user interface can only be run on a Windows machine.
    ```bash
    dotnet run --project HarborFlow.Wpf
    ```

### 2.3. Platform-Specific Development

While the UI is Windows-only, the core logic is cross-platform. Developers on **macOS or Linux** can build, test, and contribute to the following projects:
- `HarborFlow.Core`
- `HarborFlow.Application`
- `HarborFlow.Infrastructure`

For more details, see the [WINDOWS_DEVELOPMENT.md](WINDOWS_DEVELOPMENT.md) file.

## 3. System Architecture

HarborFlow is built on **Clean Architecture** principles, creating a separation of concerns that makes the system testable, independent of external frameworks, and easier to maintain.

### 3.1. Project Layers

- **`HarborFlow.Core` (Domain):** The heart of the application. Contains all business models (entities) and application-specific interfaces (contracts). It has zero dependencies on other layers.
- **`HarborFlow.Application` (Application):** Contains the application-specific business logic. It orchestrates the domain models to perform tasks and implements the interfaces defined in Core.
- **`HarborFlow.Infrastructure` (Infrastructure):** Handles all external concerns: database access (Entity Framework), communication with third-party APIs (AIS data, RSS feeds), etc.
- **`HarborFlow.Wpf` (Presentation):** The user interface layer, built with WPF and the **Model-View-ViewModel (MVVM)** pattern. It is responsible for presenting data to the user and handling user interactions.

### 3.2. Key Architectural Patterns

- **Dependency Inversion:** Dependencies flow inwards. The UI and Infrastructure layers depend on the Application and Core layers, not the other way around.
- **Dependency Injection (DI):** The application uses the built-in .NET DI container to manage dependencies, promoting loose coupling and testability. All services are registered in `App.xaml.cs`.
- **Centralized Notification Hub:** A custom `INotificationHub` service acts as a central event bus. Business logic sends notifications to this hub, and the UI subscribes to it to display messages, decoupling the backend from UI-specific notification logic.

### 3.3. Key Third-Party Libraries

The project leverages several key open-source libraries to accelerate development and provide essential functionality.

-   **`BCrypt.Net-Next`**: Used for password hashing. It securely hashes user passwords before they are stored in the database, which is a critical security practice.
-   **`FluentValidation`**: Provides a clear and powerful way to build validation rules for business objects. This ensures data integrity throughout the application.
-   **`LiveCharts.Wpf`**: The core library used for creating the interactive charts in the Analytics Dashboard.
-   **`Microsoft.Web.WebView2`**: Enables the WPF application to host web content. This is crucial for rendering the Leaflet.js interactive map within the desktop UI.
-   **`Microsoft.Xaml.Behaviors.Wpf`**: A utility library that allows for the creation of more interactive and decoupled UI behaviors in WPF.
-   **Testing Stack (`xunit`, `Moq`, `FluentAssertions`)**: The project relies on `xunit` as the test runner, `Moq` for creating mock objects to isolate dependencies, and `FluentAssertions` for writing more readable and expressive test assertions.

### 3.4 Core Data Models

The domain layer of the application is built around a set of core entities that represent the key concepts in the port management system.

-   **`User`**: Represents an individual who can log in to the system.
    -   **Key Properties**: `UserId`, `Username`, `PasswordHash`, `Email`, `Role`.
    -   **Description**: Stores user credentials, personal information, and their assigned role (e.g., Administrator, PortOfficer), which dictates their permissions within the application.

-   **`Vessel`**: Represents a single maritime vessel.
    -   **Key Properties**: `IMO` (Primary Key), `Mmsi`, `Name`, `VesselType`.
    -   **Description**: Contains static details about a vessel, such as its unique identifiers, name, and type (e.g., Cargo, Tanker).

-   **`VesselPosition`**: Represents a single, time-stamped geographical position of a vessel.
    -   **Key Properties**: `VesselImo` (Foreign Key), `Latitude`, `Longitude`, `PositionTimestamp`, `SpeedOverGround`.
    -   **Description**: This entity is used to store the historical and real-time track of a vessel. It has a many-to-one relationship with the `Vessel` entity.

-   **`ServiceRequest`**: Represents a request for a port service (e.g., pilotage, bunkering).
    -   **Key Properties**: `RequestId`, `VesselImo`, `RequestedBy` (UserId), `ServiceType`, `Status`.
    -   **Description**: This is a central entity for the service workflow, tracking the type of service needed, for which vessel, by whom it was requested, and its current status in the approval process.

-   **`MapBookmark`**: Represents a user-saved geographical area on the map.
    -   **Key Properties**: `Name`, `UserId`, `North`, `South`, `East`, `West`.
    -   **Description**: Allows authenticated users to save and quickly return to specific regions on the map. Each bookmark is tied to a specific user.

-   **`NewsArticle`**: A transient model (not stored in the database) used to represent a single article fetched from an RSS feed.
    -   **Key Properties**: `Title`, `Link`, `Description`, `PublishDate`.

-   **Supporting Enums**: The models are supported by various enums like `UserRole`, `VesselType`, `ServiceType`, and `RequestStatus` to provide clear, predefined options for specific fields.

### 3.5 WPF MVVM Implementation Details

To support the Model-View-ViewModel (MVVM) pattern effectively, the `HarborFlow.Wpf` project includes several key helper components in the `Commands`, `Services`, and `Converters` directories.

-   **Commands (`RelayCommand` & `AsyncRelayCommand`)**:
    -   **Purpose**: These classes are implementations of the `ICommand` interface. They act as a bridge between the UI (e.g., a button click) and the logic in the `ViewModel`.
    -   **`RelayCommand`**: Used for synchronous operations that complete instantly.
    -   **`AsyncRelayCommand`**: A custom implementation for asynchronous operations (e.g., fetching data from a service). It includes logic to prevent a command from being executed again while it's already running, providing built-in protection against double-clicks.

-   **Custom UI Services**:
    -   **`IWindowManager`**: This service abstracts away the logic for creating, showing, and managing windows and dialogs (e.g., `LoginView`, `VesselEditorView`). By using this service in the `ViewModel`, we avoid direct references to UI-specific window classes, which makes the `ViewModel` more testable and independent of the `View`.
    -   **`ISettingsService`**: Manages user-specific application settings, such as the selected theme (Light/Dark). It handles loading these settings from a `usersettings.json` file and saving them back.
    -   **`IFileService`**: Provides an abstraction for file-related operations, such as saving documents to a dedicated application folder.

-   **Value Converters**:
    -   **Purpose**: Converters are small classes that translate data from one format to another directly in the XAML binding. This keeps the `ViewModel` clean of UI-specific presentation logic.
    -   **`BooleanToVisibilityConverter`**: A common WPF converter that translates a `true`/`false` value into `Visibility.Visible`/`Visibility.Collapsed`, used to easily show or hide UI elements based on a condition in the `ViewModel`.
    -   **`CountToVisibilityConverter`**: Similar to the above, but it shows an element only if a collection's count is greater than zero.

## 4. Feature Implementation Details

### 4.1. Guest Mode & Authentication Flow

- The application starts directly into `MainWindow` in a "Guest" state.
- UI elements and navigation items are dynamically shown or hidden based on the user's login status (`IsLoggedIn` / `IsGuest` properties in `MainWindowViewModel`).
- The `SessionContext` service manages the current user's state. A `UserChanged` event is fired on login/logout, which the `MainWindowViewModel` subscribes to in order to refresh the UI automatically.
- Login and Register windows are shown as modal dialogs over the main window.

### 4.2. Analytics Dashboard

- The dashboard uses the `LiveCharts.Wpf` library to display data.
- `DashboardViewModel` is responsible for fetching data from services and transforming it into `SeriesCollection` objects that the charts can consume.
- **Charts Implemented:**
    - **Pie Chart:** Shows the distribution of service requests by their current status.
    - **Column Chart:** Shows the total number of vessels grouped by their type.

### 4.3. Map Bookmarks

- **Data Model:** A `MapBookmark` entity exists in `HarborFlow.Core` and is stored in a dedicated `map_bookmarks` table in the database.
- **Backend:** `BookmarkService` provides CRUD operations for bookmarks, ensuring that users can only access their own.
- **Frontend:** `MapViewModel` handles the UI logic. It fetches bookmarks when a user logs in and provides commands for adding, deleting, and navigating to a saved bookmark view. The bookmark controls are only visible when a user is logged in.

### 4.4. Maritime News Feed

- **Configuration:** A list of curated RSS feed URLs is stored in `appsettings.json` under the `RssFeeds` section.
- **Backend:** `RssService` uses `System.ServiceModel.Syndication` to fetch and parse XML data from the feed URLs.
- **Filtering Logic:** `NewsViewModel` performs two levels of filtering:
    1.  **Automatic Keyword Filter:** Only articles whose title or description contain predefined maritime keywords (e.g., "shipping", "port", "kapal") are kept.
    2.  **User Search Filter:** A search box in the UI allows users to further filter the displayed articles in real-time.
- **UI:** The `NewsView` displays articles in a modern, card-based layout.

### 4.5. Interactive Map & Data Layers

The application provides an interactive map for real-time vessel tracking, built using a combination of web technologies integrated into both the WPF and Web platforms.

-   **Core Technology:** The map is powered by the **Leaflet.js** library, a popular open-source solution for mobile-friendly interactive maps.
-   **Data Layers:** To provide rich visual context, the map uses several tile layers that users can switch between:
    -   **Street View (Default):** This layer is provided by **OpenStreetMap**, offering a detailed and free-to-use street map. It is the default view for the application.
    -   **Satellite View:** Provided by **Esri World Imagery**, this layer shows high-resolution satellite imagery.
    -   **Nautical View:** Provided by **OpenSeaMap**, this layer overlays nautical charts and maritime-specific information, which is essential for port operations.
-   **Implementation:**
    -   In the **WPF application**, the map is rendered inside a `WebView2` control, loading a local HTML file (`wwwroot/map/index.html`) that contains the Leaflet.js setup.
    -   In the **Blazor Web application**, the `ST.Blazor.Leaflet` component is used to integrate the map directly into the Razor page (`Pages/MapView.razor`).

### 4.6. External API Integration (AIS Data)

A core feature of HarborFlow is its ability to track vessels in real-time. This is achieved by integrating with a third-party AIS (Automatic Identification System) data provider.

-   **`AisStreamService` (Real-time Streaming):**
    -   **Provider:** AISstream (`stream.aisstream.io`)
    -   **Purpose:** This service provides real-time position updates for vessels across the globe.
    -   **Implementation:** It establishes a persistent WebSocket connection to the AISstream server. After connecting, it sends a subscription message for a global bounding box and then continuously listens for incoming position reports. When a new position is received, it raises a `PositionReceived` event that other parts of the application can subscribe to.
    -   **Configuration:** Requires an API key to be configured in `appsettings.json` under `ApiKeys:AisStream`.

The `AisDataService` currently returns placeholder data, as the previous non-free data source has been removed. It can be integrated with a detailed vessel data provider in the future.

## 5. Future Development Roadmap

- **Phase 2: Advanced Map Features**
    - **Geofencing:** Allow users to draw zones on the map and receive alerts when vessels enter or exit.
    - **Vessel History Playback:** Animate the historical track of a selected vessel on the map.
- **Phase 3: Architectural Evolution**
    - **Decouple to Web API:** Refactor the business and data logic into a separate ASP.NET Core Web API project. The WPF application would then become a pure client, improving scalability and preparing for future web/mobile versions.
- **Phase 4: Enhanced Collaboration**
    - **Shared Bookmarks:** Allow users within the same organization to share map bookmarks.
    - **Real-time Notifications with SignalR:** Replace the current event-based system with a full SignalR implementation for instant, server-pushed updates.
