# Story 4.1: Service Request Status Visualization

Status: completed

## Description
As a **Port Authority** or **System Admin**, I want to see a visual breakdown of Service Requests by their status (Pending, In Progress, Completed, Rejected) on the Dashboard, so that I can quickly assess the current workload and identify bottlenecks.

## Acceptance Criteria
- [x] **Chart Component:** Implement a chart (e.g., Pie Chart or Bar Chart) using `MudBlazor` charts.
- [x] **Data Source:** Fetch service request data from the backend, grouped by status.
- [x] **Real-time/Reactive:** The chart should reflect the latest data (initially on load, real-time updates in later stories).
- [x] **Interactive:** Hovering over chart segments should show the exact count and percentage.
- [x] **Empty State:** Handle cases where there are no service requests gracefully.

## Implementation Details
-   **Frontend:** Added `MudChart` (Pie Chart) to `Dashboard.razor`.
-   **Backend:** Leveraged existing `DashboardController.GetServiceRequestStatusSummary` endpoint.
-   **Data:** Visualizes counts of Pending, In Progress, Completed, and Rejected requests.

## Technical Notes
-   Use `MudChart` component.
-   Create a new `DashboardService` or extend `ServiceRequestService` to provide aggregated stats (e.g., `GetServiceRequestStats()`).
-   Endpoint: `GET /api/servicerequest/stats` (or similar).

## Dependencies
-   Epic 3 (RBAC) must be complete to ensure data isolation (users only see their own company's data or all data based on role).
