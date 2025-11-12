# Epics and Stories for HarborFlow Suite

This document outlines the epics and user stories for the HarborFlow Suite project. It is a living document that should be updated as new requirements are discovered during design and development.

---

## Epic 1: Real-time Vessel Tracking System

*   **Story 1.1: Display Interactive Map with Vessel Positions**
    *   **Description:** As a user, I want to see an interactive map displaying real-time vessel positions so that I can monitor maritime activity.
    *   **Acceptance Criteria:**
        *   The map loads within 2 seconds.
        *   Vessels are displayed with accurate coordinates.
        *   The map is responsive across desktop, tablet, and mobile devices.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-3.1, FR-3.5

*   **Story 1.2: Real-time Position Updates**
    *   **Description:** As a user, I want vessel positions to update in real-time via SignalR so that I always have the most current information.
    *   **Acceptance Criteria:**
        *   Position updates propagate to all connected clients within 1 second.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-3.2
    *   **Dependencies:** Story 1.1

*   **Story 1.3 (Revised): Implement Vessel Detail Sliding Panel**
    *   **Description:** As a user, I want to view detailed information about a vessel in a sliding panel from the right by clicking on its icon on the map, without losing the map context.
    *   **Acceptance Criteria:**
        *   Clicking a vessel displays a sliding panel from the right with vessel details.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-3.3
    *   **Dependencies:** Story 1.1

*   **Story 1.4: Multiple Map Layer Support**
    *   **Description:** As a user, I want to switch between different map views (e.g., Street, Satellite) to customize my map experience.
    *   **Acceptance Criteria:**
        *   Users can switch between Street, Satellite, and other map views.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-3.4
    *   **Dependencies:** Story 1.1

*   **Story 1.5 (New): Implement Collapsible Sidebar for Map View**
    *   **Description:** As a user, I want to be able to collapse the main sidebar to have an immersive, full-screen map experience.
    *   **Acceptance Criteria:**
        *   The sidebar can be collapsed and expanded with a single click.
        *   The map content reflows to fill the available space.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 1.1

*   **Story 1.6 (New): Implement Vessel Hover Tooltip**
    *   **Description:** As a user, I want to see a vessel's name and speed in a small tooltip when I hover over its icon for quick identification.
    *   **Acceptance Criteria:**
        *   Hovering over a vessel icon displays a tooltip with its name and speed.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 1.1

*   **Story 1.7 (New): Implement Map Search Highlighting**
    *   **Description:** As a user searching for a vessel, I want non-matching vessels on the map to dim and matching vessels to be highlighted in real-time as I type.
    *   **Acceptance Criteria:**
        *   The map visually differentiates search results as the user types.
    *   **Priority:** Should-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 1.1

*   **Story 1.8 (New): Handle Live Feed Interruption State**
    *   **Description:** As a user, I want to be clearly informed when the live data feed is interrupted so I don't act on stale data.
    *   **Acceptance Criteria:**
        *   A non-intrusive banner appears when the data feed is down.
        *   Vessel icons change appearance (e.g., turn grey) to indicate their data is not live.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 1.2

## Epic 2: User Authentication & Authorization System

*   **Story 2.1: Firebase Authentication Integration**
    *   **Description:** As a user, I want to be able to register and log in using email/password and social providers via Firebase Authentication.
    *   **Acceptance Criteria:**
        *   Users can successfully register and log in with email/password.
        *   Users can successfully log in with supported social providers.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-1.1

*   **Story 2.2: JWT Token Validation**
    *   **Description:** As a system, all API requests must validate Firebase JWT tokens to ensure secure access.
    *   **Acceptance Criteria:**
        *   All API requests successfully validate Firebase JWT tokens.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-1.2
    *   **Dependencies:** Story 2.1

*   **Story 2.3: Session Management**
    *   **Description:** As a user, I want my session to persist across browser sessions with automatic token refresh for a seamless experience.
    *   **Acceptance Criteria:**
        *   User sessions persist across browser sessions.
        *   Tokens are automatically refreshed.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-1.3
    *   **Dependencies:** Story 2.1

*   **Story 2.4: User Profile Management**
    *   **Description:** As a user, I want to view and update my basic profile information.
    *   **Acceptance Criteria:**
        *   Users can view their profile information.
        *   Users can update basic profile information.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-1.4
    *   **Dependencies:** Story 2.1

*   **Story 2.5: Account Recovery**
    *   **Description:** As a user, I want to be able to reset my password via email if I forget it.
    *   **Acceptance Criteria:**
        *   Password reset functionality via email is available and works.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-1.5
    *   **Dependencies:** Story 2.1

## Epic 3: Role-Based Access Control (RBAC) System

*   **Story 3.1: Four-Tier Role System Implementation**
    *   **Description:** As a system, I need to implement a four-tier role system (System Administrator, Port Authority Officer, Vessel Agent, Guest) to manage user access.
    *   **Acceptance Criteria:**
        *   System Administrator, Port Authority Officer, Vessel Agent, and Guest roles function correctly.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-2.1

*   **Story 3.2: Granular Permission Enforcement**
    *   **Description:** As a system, I need to enforce granular permissions (e.g., `vessel:read:all`, `servicerequest:approve`) at the API level to control access to resources.
    *   **Acceptance Criteria:**
        *   All specified permissions are enforced at the API level.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-2.2
    *   **Dependencies:** Story 3.1

*   **Story 3.3: Company-Based Data Isolation**
    *   **Description:** As a Vessel Agent, I only want to access data relevant to my company.
    *   **Acceptance Criteria:**
        *   Vessel Agents can only access their company's data.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-2.3
    *   **Dependencies:** Story 3.1

*   **Story 3.4: Role Assignment Management**
    *   **Description:** As an Administrator, I want to be able to assign and modify user roles.
    *   **Acceptance Criteria:**
        *   Administrators can assign user roles.
        *   Administrators can modify user roles.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-2.4
    *   **Dependencies:** Story 3.1

*   **Story 3.5: Permission Inheritance**
    *   **Description:** As a system, roles should inherit appropriate permission sets automatically.
    *   **Acceptance Criteria:**
        *   Roles inherit appropriate permission sets automatically.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 3.1

## Epic 4: Analytics Dashboard

*   **Story 4.1: Service Request Status Visualization**
    *   **Description:** As a Port Authority Officer, I want to see a chart displaying pending, approved, and rejected service request counts to monitor operational workflow.
    *   **Acceptance Criteria:**
        *   A chart displays pending, approved, and rejected request counts.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-8.1, FR-8.2, FR-8.3

*   **Story 4.2: Vessel Count by Type Analytics**
    *   **Description:** As a Port Authority Officer, I want to see a chart showing vessel distribution by type/category for fleet overview.
    *   **Acceptance Criteria:**
        *   A chart shows vessel distribution by type/category.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-8.3
    *   **Dependencies:** Story 4.1

*   **Story 4.3: Role-Based Data Filtering**
    *   **Description:** As a user, I want to see analytics relevant to my permissions and company.
    *   **Acceptance Criteria:**
        *   Users see analytics relevant to their permissions and company.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-9.1
    *   **Dependencies:** Story 4.1

*   **Story 4.4: Real-time Dashboard Updates**
    *   **Description:** As a user, I want the dashboard charts to update automatically when underlying data changes.
    *   **Acceptance Criteria:**
        *   Charts update automatically when underlying data changes.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-10.1
    *   **Dependencies:** Story 4.1

*   **Story 4.5: Export Functionality**
    *   **Description:** As a user, I want to export chart data in common formats (CSV, PDF).
    *   **Acceptance Criteria:**
        *   Users can export chart data in CSV format.
        *   Users can export chart data in PDF format.
    *   **Priority:** Could-Have
    *   **FR-Reference:** FR-11.1
    *   **Dependencies:** Story 4.1

## Epic 5: Service Request Management System

*   **Story 5.1 (Revised): Implement Contextual Modal Service Request Form**
    *   **Description:** As a Vessel Agent, I want to submit service requests using a modal form that is pre-populated with vessel data when launched from the map context.
    *   **Acceptance Criteria:**
        *   Users can submit requests with required fields.
        *   Form fields have appropriate validation.
        *   Launching from a vessel's detail panel pre-populates the vessel ID.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-5.1

*   **Story 5.2: Request Status Tracking**
    *   **Description:** As a user, I want to view the current status and approval history of my service requests using colored chips for easy scanning.
    *   **Acceptance Criteria:**
        *   Users can view the current status of their requests.
        *   Users can view the approval history of their requests.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-6.2, FR-6.3
    *   **Dependencies:** Story 5.1

*   **Story 5.3: Company-Based Request Filtering**
    *   **Description:** As a user, I want to see only service requests relevant to my company/role.
    *   **Acceptance Criteria:**
        *   Users see only requests relevant to their company/role.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-7.1
    *   **Dependencies:** Story 5.1

*   **Story 5.4 (Revised): Implement Badge and Toast Notifications for Service Requests**
    *   **Description:** As a user, I want to receive notifications on service request status changes via sidebar badges and on-screen toasts.
    *   **Acceptance Criteria:**
        *   A badge on the sidebar icon shows the count of pending requests.
        *   A toast notification appears when a request is submitted or its status changes.
    *   **Priority:** Should-Have
    *   **FR-Reference:** FR-7.2
    *   **Dependencies:** Story 5.1

*   **Story 5.5 (New): Build Two-Pane Review Command Center Layout**
    *   **Description:** As a Port Authority Officer, I need a two-pane layout for reviewing service requests, with a queue on the left and details on the right.
    *   **Acceptance Criteria:**
        *   The UI is structured as a two-pane layout.
        *   Selecting an item in the left pane displays its details in the right pane.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 5.1

*   **Story 5.6 (New): Implement Request Detail Pane with Mini-Map**
    *   **Description:** As a Port Authority Officer, I want the request detail pane to include a mini-map showing the vessel and requested service location for spatial context.
    *   **Acceptance Criteria:**
        *   The detail pane shows all request information.
        *   A mini-map is present and displays relevant locations.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 5.5

*   **Story 5.7 (New): Implement Approve/Reject Actions with Keyboard Shortcuts**
    *   **Description:** As a Port Authority Officer, I want to approve or reject requests with a single click or keypress (A/R) and have the queue advance automatically.
    *   **Acceptance Criteria:**
        *   Officers can approve requests.
        *   Officers can reject requests with a required reason.
        *   The queue automatically advances to the next item after an action.
        *   Keyboard shortcuts (A/R) are functional.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-6.1
    *   **Dependencies:** Story 5.5

## Epic 6: Maritime News Aggregation

*   **Story 6.1: Curated Maritime News Feed**
    *   **Description:** As a user, I want to view a curated maritime industry news feed.
    *   **Acceptance Criteria:**
        *   A news feed displays relevant maritime industry news.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-12.1

*   **Story 6.2: Client-Side Filtering**
    *   **Description:** As a user, I want to filter the news feed by category and keywords.
    *   **Acceptance Criteria:**
        *   Users can filter news by category.
        *   Users can filter news by keywords.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-12.2
    *   **Dependencies:** Story 6.1

## Epic 7: Database Schema & Data Management

*   **Story 7.1: PostgreSQL Database Schema**
    *   **Description:** As a system, I need a PostgreSQL database schema that supports all application entities with proper relationships and constraints.
    *   **Acceptance Criteria:**
        *   The PostgreSQL database schema supports all application entities.
        *   Proper relationships and constraints are defined.
    *   **Priority:** Critical
    *   **FR-Reference:** Foundational (underpins all data-related FRs)

## Epic 8: Map Bookmarking System

*   **Story 8.1: Save Map Locations**
    *   **Description:** As a user, I want to save and quickly return to specific map locations.
    *   **Acceptance Criteria:**
        *   Users can save map locations.
        *   Users can quickly navigate to saved map locations.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-13.1

## Epic 9: Progressive Web App (PWA) Capabilities

*   **Story 9.1: Offline Functionality**
    *   **Description:** As a user, I want the application to function offline, allowing me to access cached data and perform limited actions.
    *   **Acceptance Criteria:**
        *   The application loads and displays cached data when offline.
        *   Users can perform limited actions (e.g., view news feed) when offline.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-14.1

*   **Story 9.2: Background Synchronization**
    *   **Description:** As a user, I want data entered offline to synchronize with the server once an internet connection is restored.
    *   **Acceptance Criteria:**
        *   Offline data synchronizes with the server when online.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-14.2
    *   **Dependencies:** Story 9.1

*   **Story 9.3: Push Notifications**
    *   **Description:** As a user, I want to receive push notifications for important updates (e.g., service request status changes).
    *   **Acceptance Criteria:**
        *   Users receive push notifications for important updates.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-14.3
    *   **Dependencies:** Story 9.1

## Epic 10: Global Command Palette

*   **Story 10.1 (Revised): Implement Command Palette UI**
    *   **Description:** As a user, I want to access a global command palette via keyboard shortcuts (Cmd+K/Ctrl+K) that appears as a centered modal.
    *   **Acceptance Criteria:**
        *   The command palette opens with Cmd+K/Ctrl+K.
        *   The UI appears as a centered modal with a search input.
    *   **Priority:** Must-Have
    *   **FR-Reference:** FR-15.1

*   **Story 10.2 (New): Develop Multi-Entity Fuzzy Search Engine**
    *   **Description:** As a user, I want the command palette to instantly search across vessels, service requests, and actions as I type, with results grouped by category.
    *   **Acceptance Criteria:**
        *   Search is performed on every keystroke.
        *   Search includes multiple data types (vessels, requests, etc.).
        *   Results are grouped by category.
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 10.1

*   **Story 10.3 (New): Implement Command Palette Keyboard Navigation and Action Execution**
    *   **Description:** As a user, I want to navigate command palette results with arrow keys and execute the selected item with the Enter key.
    *   **Acceptance Criteria:**
        *   Users can navigate results with Up/Down arrow keys.
        *   Pressing Enter on a result executes the associated action (e.g., navigates to a page, opens a modal).
    *   **Priority:** Must-Have
    *   **FR-Reference:** New Requirement
    *   **Dependencies:** Story 10.2

## Epic 11 (New): UI/UX Foundation

*   **Story 11.1 (New): Implement MudBlazor and Base Theme**
    *   **Description:** As a developer, I need to set up the MudBlazor design system and apply the "High-Tech Command" dark theme with the "Nautical Professional" color palette.
    *   **Acceptance Criteria:**
        *   MudBlazor is installed and configured.
        *   The application uses the specified dark theme and colors by default.
    *   **Priority:** Critical
    *   **FR-Reference:** Foundational (underpins all UI/UX related FRs)

*   **Story 11.2 (New): Implement Core Application Layout**
    *   **Description:** As a user, I want the main application shell to feature a collapsible sidebar for navigation and a main content area.
    *   **Acceptance Criteria:**
        *   The main layout consists of a collapsible sidebar and a content area.
    *   **Priority:** Critical
    *   **FR-Reference:** Foundational (underpins all UI/UX related FRs)
    *   **Dependencies:** Story 11.1

*   **Story 11.3 (New): Define and Implement Typography & Spacing System**
    *   **Description:** As a developer, I need to configure the application's CSS to use the "Inter" font, a standard type scale, and an 8px spacing grid.
    *   **Acceptance Criteria:**
        *   The "Inter" font is applied correctly.
        *   CSS variables for the spacing system are available and used.
    *   **Priority:** Critical
    *   **FR-Reference:** Foundational (underpins all UI/UX related FRs)
    *   **Dependencies:** Story 11.1