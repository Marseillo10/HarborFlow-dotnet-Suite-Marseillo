# Implementation Plan - Stories 5.5, 5.6, 5.7:### Story 5.5: Two-Pane Review Layout
- [x] Create `ServiceRequestDetail` component.
- [x] Refactor `ServiceRequestManagement` to use `MudGrid` with list and detail view.
- [x] Implement selection logic.

### Story 5.6: Detail Pane with Mini-Map
- [x] Create `VesselMiniMap` component.
- [x] Integrate into `ServiceRequestDetail`.
- [x] Implement map logic in `map.js`.

### Story 5.7: Quick Actions
- [x] Add Approve/Reject buttons to `ServiceRequestDetail`.
- [x] Implement keyboard shortcuts (A/R).
- [x] Implement auto-advance logic in `ServiceRequestManagement`.es

### Frontend

#### [MODIFY] [ServiceRequestManagement.razor](file:///Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo/HarborFlowSuite/HarborFlowSuite.Client/Pages/ServiceRequestManagement.razor)
- **Layout:** Switch to `MudGrid`.
- **Left Pane:** `MudTable` (simplified columns: Title, Status, Priority).
    -   OnRowClick: Selects the request.
- **Right Pane:** Render `ServiceRequestDetail` component.

#### [NEW] [ServiceRequestDetail.razor](file:///Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo/HarborFlowSuite/HarborFlowSuite.Client/Components/ServiceRequestDetail.razor)
- **Content:** Displays full request details.
- **Mini-Map:** Shows vessel location (Story 5.6).
- **Actions:** Approve/Reject buttons (Story 5.7).
- **Keyboard Shortcuts:** Listen for 'A' (Approve) and 'R' (Reject) keys (Story 5.7).

## Verification Plan

### Manual Verification
1.  **Log in as Port Authority Officer**.
2.  **Navigate to Service Request Management**.
3.  **Verify Layout:** Two panes are visible.
4.  **Action:** Click a request in the left list.
5.  **Verify:** Details appear in the right pane.
6.  **Verify:** Mini-map shows the correct location.
7.  **Action:** Press 'A' (or click Approve).
8.  **Verify:** Approval dialog opens (or action is performed).
9.  **Verify:** After action, the next request in the list is automatically selected.
