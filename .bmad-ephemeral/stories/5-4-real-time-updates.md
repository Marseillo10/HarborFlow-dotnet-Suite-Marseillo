# Story 5.4: Real-Time Updates

**As a** user,
**I want** to see service request updates immediately without refreshing the page,
**So that** I can react quickly to changes in status or new requests.

## Acceptance Criteria
- [x] **Real-Time Notification:** When a service request is created, updated, or deleted, all connected clients receive a notification.
- [x] **Dashboard Update:** The "Pending Requests" counter on the Dashboard updates automatically.
- [x] **List Update:** The Service Request Management list refreshes automatically when an update occurs.
- [x] **Technical Implementation:** Uses SignalR for real-time communication.

## Implementation Details
-   **Backend:**
    -   Create `ServiceRequestHub`.
    -   Inject `IHubContext` into `ServiceRequestService`.
    -   Broadcast "ReceiveRequestUpdate" message on Create/Update/Delete.
-   **Frontend:**
    -   Create `ServiceRequestSignalRService` to manage connection.
    -   Subscribe to events in `Dashboard.razor` and `ServiceRequestManagement.razor`.
