# Story 5.7: Quick Actions

## Goal
Streamline the review process for Port Authority Officers by enabling keyboard shortcuts for approving and rejecting service requests, and automatically advancing to the next request.

## Acceptance Criteria
- [x] Add "Approve" (A) and "Reject" (R) buttons to the `ServiceRequestDetail` pane.
- [x] Implement keyboard shortcuts: 'A' for Approve, 'R' for Reject.
- [x] Ensure shortcuts only trigger when not typing in an input field.
- [x] Only visible/active for users with the "Port Authority Officer" role.
- [x] Only active for requests in "Pending" status.
- [x] After an action is taken, automatically select the next request in the list.

## Implementation Details
- **Component**: `ServiceRequestDetail.razor` (Buttons, Logic, Shortcuts), `ServiceRequestManagement.razor` (Auto-advance).
- **Shortcuts**: Implemented via `shortcuts.js` and JS Interop.
- **Auto-Advance**: Logic in `ServiceRequestManagement.razor` to find and select the next item in the filtered list.
