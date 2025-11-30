# Story 5.5: Two-Pane Review Layout

**As a** Port Authority Officer,
**I need** a two-pane layout for reviewing service requests, with a queue on the left and details on the right,
**So that** I can efficiently process requests without navigating back and forth.

## Acceptance Criteria
- [ ] **Two-Pane Layout:** The Service Request Management page (for Officers) displays a list on the left and details on the right.
- [ ] **Selection:** Clicking a request in the left list loads its details in the right pane.
- [ ] **Responsive Design:** On smaller screens, it might collapse to a single pane (list first, then details), but the primary requirement is for desktop efficiency.

## Implementation Details
-   **Frontend:**
    -   Modify `ServiceRequestManagement.razor`.
    -   Use `MudGrid` or CSS Grid for layout.
    -   Left Pane: `MudList` or `MudTable` (simplified).
    -   Right Pane: `ServiceRequestDetail` component (new or refactored).
    -   State: `_selectedRequest` to track the active item.
