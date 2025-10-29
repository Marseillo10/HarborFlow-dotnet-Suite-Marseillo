# Project Memory Bank

This document serves as a log of key architectural decisions, refactoring efforts, and feature implementations made during the development of HarborFlow.

## Web Application Development (October 2025)

- **Decision:** A fully functional Blazor Server web application was built out to serve as the primary, cross-platform front-end.
- **Reasoning:** To provide modern, web-based access to all core features, complementing the existing WPF application.
- **Key Implementations & Decisions:**
    - **Full Feature Parity:** All major features from the WPF application were implemented, including the Dashboard, Vessel Management, Service Requests, and an enhanced News Feed.
    - **Real-Time Map:** The map was made fully interactive and live. It now subscribes to the backend `AisStreamService` and uses JS Interop to create and move vessel markers in real-time as new data arrives.
    - **Advanced News Feed:** The news feature was completely overhauled with multi-category tabs, interactive date filters, auto-scrolling, a 3-column card layout, and a much more robust backend keyword filter to ensure content relevance.
    - **Guest Access Model:** The application was refactored to allow full public access to all pages. Functionality requiring a user account (saving, editing, approving) is now dynamically disabled on the UI for guest users, preventing runtime errors.
    - **Guest Bookmarking:** To provide a better UX for guests, a separate bookmarking system was implemented using the browser's `localStorage`, completely distinct from the database-backed system used by logged-in users. This feature was later removed to align with the goal of providing a consistent experience for all users.
    - **Debugging & Stability:** A significant portion of the session was dedicated to a deep debugging process, resolving dozens of build and runtime errors related to dependency injection, configuration, incorrect method/property names, and Blazor component lifecycle issues.


## Major Refinements & Decisions

### 1. Database Environment: Dockerization

- **Decision:** Migrated the database setup from a local PostgreSQL installation to a containerized environment using Docker Compose.
- **Reasoning:** To ensure a consistent, isolated, and easy-to-set-up database environment for all developers, regardless of their operating system. This resolves common "it works on my machine" issues related to database configuration.
- **Implementation:**
    - Created a `docker-compose.yml` file to define the `postgres` service.
    - Updated the connection string in `appsettings.json` to point to the Docker container's port (`5433`).
    - Added a `HarborFlowDbContextFactory` to enable `dotnet ef` tools to work correctly from the command line.

### 2. Application Flow: Guest Mode & In-App Authentication

- **Decision:** Refactored the application startup flow. Instead of forcing a login screen, the app now starts directly into the main window in a "Guest" mode.
- **Reasoning:** To provide a more modern, user-friendly experience, allowing users to explore public features before committing to registration. This follows the "freemium" model common in modern applications.
- **Implementation:**
    - Changed the startup view in `App.xaml.cs` from `LoginView` to `MainWindow`.
    - Implemented `IsLoggedIn` and `IsGuest` properties in `MainWindowViewModel` to dynamically control UI element visibility.
    - Created a `UserChanged` event in `SessionContext` to allow the UI to react automatically to login/logout events.
    - Modified `LoginViewModel` to close itself as a dialog instead of opening a new main window.

### 3. UI/UX Overhaul

- **Decision:** Redesigned key views to be more visually appealing and intuitive.
- **Implementation:**
    - **Authentication Screens:** Applied a professional, full-image background (`AuthorizationBackground.jpg`) to `LoginView` and `RegisterView`, with a semi-transparent panel to ensure form readability.
    - **Dashboard:** Replaced static data lists with interactive charts from the `LiveCharts.Wpf` library. Added visual cards with vector icons (`Icons.xaml`) for key metrics.

### 4. Feature Implementation: Maritime News Feed

- **Decision:** Added a new feature to aggregate and display maritime news from various sources.
- **Reasoning:** To enrich the application's value as a central information portal for port stakeholders.
- **Implementation:**
    - **Backend:** Created `RssService` to fetch and parse XML data from a configurable list of RSS feeds in `appsettings.json`.
    - **Filtering:** Implemented a dual-filtering mechanism in `NewsViewModel`:
        1.  Automatic filtering based on a predefined list of maritime keywords.
        2.  A user-facing search box for real-time manual filtering.
    - **UI:** Designed a modern, card-based layout in `NewsView.xaml` to display articles.

### 5. Feature Implementation: Map Bookmarks

- **Decision:** Implemented a feature allowing logged-in users to save and navigate to specific map locations.
- **Reasoning:** To improve user efficiency by providing quick access to frequently viewed areas.
- **Implementation:**
    - **Data Layer:** Added a `MapBookmark` model and a corresponding `map_bookmarks` table to the database via an EF migration.
    - **Backend:** Created `BookmarkService` to handle all CRUD operations for bookmarks.
    - **UI:** Added a floating control panel to `MapView` containing a dropdown list of saved bookmarks and buttons to add/delete them. The panel's visibility is tied to the user's login state.

### 6. Architectural Refactoring: Notification System

- **Decision:** Centralized the application's notification logic.
- **Reasoning:** To decouple the business logic from the UI presentation and create a single source of truth for all user-facing notifications. This prepares the architecture for a future move to a real-time system like SignalR.
- **Implementation:**
    - Created a singleton `INotificationHub` service to act as a central event bus.
    - Modified business services (e.g., `PortServiceManager`) to send notifications to the hub instead of directly to a UI service.
    - Modified `MainWindowViewModel` to subscribe to the hub and be the sole component responsible for displaying toast notifications.
    - Removed all direct calls to `INotificationService` from other ViewModels.

### 7. API Refactoring: Vessel Detail Provider

- **Decision:** Replaced the `VesselFinder` API with the `Global Fishing Watch` API as the primary source for detailed vessel data.
- **Reasoning:** The original `VesselFinder` API was not entirely free. After analysis, the `Global Fishing Watch` API was identified as a suitable alternative that provides a free tier for non-commercial use, which aligns with the academic scope of this project.
- **Implementation:**
    - The `AisDataService` was refactored to call the Global Fishing Watch API endpoint.
    - A new DTO, `GfwVesselResponse.cs`, was created to handle the API's JSON response.
    - A new configuration key, `ApiKeys:GlobalFishingWatch`, was added to `appsettings.json` to store the required API token.