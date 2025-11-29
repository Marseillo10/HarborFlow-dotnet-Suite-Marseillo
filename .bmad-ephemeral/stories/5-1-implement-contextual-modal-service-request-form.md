# Story 5.1: Implement Contextual Modal Service Request Form

**As a** Port Authority or Vessel Agent,
**I want** to create service requests via a modal dialog without leaving my current view (e.g., the map),
**So that** I can maintain context and workflow efficiency.

## Acceptance Criteria
- [x] **Modal Creation:** A modal dialog replaces the separate "Add Service Request" page.
- [x] **Context Awareness:** If triggered from a vessel on the map, the "Vessel" field is pre-filled.
- [x] **Form Validation:** The modal includes validation for required fields (Title, Description, Type, Priority).
- [x] **Submission:** Successfully submitting the form closes the modal and shows a success notification.
- [x] **Cancellation:** Clicking "Cancel" or outside the modal closes it without saving.

## Implementation Details
-   **Frontend:**
    -   Create `ServiceRequestDialog.razor` using `MudDialog`.
    -   Update `ServiceRequestManagement.razor` to open this dialog instead of navigating.
    -   Update `VesselMap.razor` (or tooltip) to add a "Create Request" button that opens this dialog with the vessel ID.
-   **Backend:**
    -   Reuse existing `POST /api/servicerequest` endpoint.
