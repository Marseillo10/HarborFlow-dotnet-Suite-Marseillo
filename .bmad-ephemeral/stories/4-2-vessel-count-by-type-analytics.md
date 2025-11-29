# Story 4.2: Vessel Count by Type Analytics

Status: completed

## Description
As a **Port Authority**, I want to see a breakdown of vessels by type (e.g., Cargo, Tanker) currently in the port, so that I can understand the traffic composition.

## Acceptance Criteria
- [x] **Chart Component:** Implement a Donut Chart on the Dashboard.
- [x] **Data Source:** Fetch vessel type counts from the backend.
- [x] **Real-time:** Updates when vessel data changes (via SignalR).
- [x] **Filtering:** Respects company isolation (only shows vessels for the user's company).

## Implementation Details
-   Implemented in `Dashboard.razor` using `MudChart` (Donut).
-   Backend endpoint `GET /api/dashboard/vesseltypes` provided by `DashboardController`.
-   `DashboardService` aggregates data from `Vessels` table.
