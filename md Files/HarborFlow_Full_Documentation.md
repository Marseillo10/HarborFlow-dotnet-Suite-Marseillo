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

## 5. Future Development Roadmap

- **Phase 2: Advanced Map Features**
    - **Geofencing:** Allow users to draw zones on the map and receive alerts when vessels enter or exit.
    - **Vessel History Playback:** Animate the historical track of a selected vessel on the map.
- **Phase 3: Architectural Evolution**
    - **Decouple to Web API:** Refactor the business and data logic into a separate ASP.NET Core Web API project. The WPF application would then become a pure client, improving scalability and preparing for future web/mobile versions.
- **Phase 4: Enhanced Collaboration**
    - **Shared Bookmarks:** Allow users within the same organization to share map bookmarks.
    - **Real-time Notifications with SignalR:** Replace the current event-based system with a full SignalR implementation for instant, server-pushed updates.
