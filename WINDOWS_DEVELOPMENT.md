# Windows-Specific & Cross-Platform Development Guide

This document outlines development tasks and limitations for both Windows and non-Windows (macOS, Linux) environments.

## Background

While the majority of the codebase (.NET Class Libraries for `Core`, `Application`, and `Infrastructure`) is cross-platform, the project includes two user interfaces:
- **`HarborFlow.Wpf`**: A Windows-only desktop application using WPF.
- **`HarborFlow.Web`**: A cross-platform web application using Blazor.

This guide clarifies what can be done on each platform.

## Cross-Platform Development (macOS, Linux, Windows)

Developers on any platform can run the web application and contribute to the core backend logic.

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
  dotnet ef database update --startup-project HarborFlow.Web --project HarborFlow.Infrastructure
  ```

### 3. Running the Web Application

The Blazor web application is the primary way to run and test the application on any platform.

- **Run the web app:**
  ```shell
  dotnet run --project HarborFlow.Web
  ```

### 4. Backend Development Cycle

Your day-to-day development will focus on the backend projects:

- **Write & Modify Code**: You can freely create and edit features in `HarborFlow.Core`, `HarborFlow.Application`, and `HarborFlow.Infrastructure`.
- **Build the Solution**: After making changes, run a build to check for compilation errors.
  ```shell
  dotnet build HarborFlow.sln
  ```
- **Run Backend Tests**: Use the dedicated backend test project to verify your changes.
  ```shell
  dotnet test HarborFlow.Backend.Tests/
  ```

## Windows-Only Tasks (WPF UI)

Any work that requires visual validation of the WPF UI must be done on a Windows machine.

### 1. Running and Debugging the WPF UI

This is the only way to visually verify that the WPF UI functions as expected.

- **Steps**: Open `HarborFlow.sln` in Visual Studio, set `HarborFlow.Wpf` as the startup project, and run. Alternatively, use the command line:
  ```shell
  dotnet run --project HarborFlow.Wpf
  ```

### 2. Manual UI/UX Testing Checklist

After launching the application on Windows, use the following checklist to manually verify the UI functionality. This checklist was generated based on a code-level analysis of the Views and ViewModels.

#### A. Core Authentication & Navigation
- **Login/Register Dialogs:**
  - [ ] Can you open the Login and Register dialogs?
  - [ ] Do they correctly appear over the main window?
  - [ ] Does a successful login close the dialog and update the main window to the authenticated state?
  - [ ] Does logging out revert the application to the guest state?
- **Main Navigation:**
  - [ ] Does clicking on each navigation item (Dashboard, Map, News) display the correct view?
  - [ ] Is view state preserved? (e.g., scroll position in News, map zoom level).

#### B. Core Feature Functionality (CRUD)
- **Vessel Management (Admin Role):**
  - [ ] **Create:** Does the "Add" button open the `VesselEditorView` dialog?
  - [ ] **Save:** Does saving a new/edited vessel correctly update the list in `VesselManagementView`?
  - [ ] **Delete:** Does deleting a vessel remove it from the list?
- **Service Requests (Port Officer/Admin Roles):**
  - [ ] **Approve/Reject:** Does approving or rejecting a request immediately update its status in the list?

#### C. Validation and User Feedback
- **Forms (Register, Vessel Editor):**
  - [ ] Do validation messages appear for empty or incorrectly formatted fields?
  - [ ] Is the "Save" button disabled until the form is valid?
- **Error Notifications:**
  - [ ] When an action fails (e.g., adding a bookmark without an internet connection), does a user-friendly notification appear?

#### D. Role-Based UI
- **Login with Different Roles:**
  - [ ] **Admin:** Can see "Vessel Management" and "Service Requests".
  - [ ] **Port Officer:** Can see "Service Requests" but NOT "Vessel Management".
  - [ ] **Maritime Agent:** Cannot see either admin menu, but CAN see the "Bookmarks" panel on the map view.

#### E. General UI Polish
- **Loading Indicators:**
  - [ ] Does a loading spinner or message appear during long operations (e.g., initial dashboard load, vessel search)?
- **Window Resizing:**
  - [ ] Do all UI elements and layouts adapt gracefully when the main window is resized?
- **Theme Switching (Light/Dark):**
  - [ ] Do all controls, text, and backgrounds update correctly when the theme is changed?
