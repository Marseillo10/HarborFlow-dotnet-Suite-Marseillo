# Story 5.2: Request Status Tracking

## Goal
Provide users with a complete audit trail of all actions taken on a service request, from creation to completion. This ensures transparency and accountability.

## Acceptance Criteria
- [ ] Record an entry in `ApprovalHistory` when a request is **Created**.
- [ ] Record an entry in `ApprovalHistory` when a request is **Edited** (already implemented, verify).
- [ ] Record an entry in `ApprovalHistory` when a request is **Approved** (already implemented).
- [ ] Record an entry in `ApprovalHistory` when a request is **Rejected** (already implemented).
- [ ] Add a "View History" button to the `ServiceRequestDetail` pane.
- [ ] The History Dialog should display all actions with appropriate timestamps, users, and comments.
- [ ] Color-code actions in the timeline (Created: Blue/Info, Edited: Grey/Default, Approved: Green/Success, Rejected: Red/Error).

## Implementation Details
- **Backend**: Update `ServiceRequestService.CreateServiceRequest` to add an `ApprovalHistory` entry.
- **Frontend**:
    - Update `ServiceRequestDetail.razor` to add the button.
    - Update `ServiceRequestHistoryDialog.razor` to handle new action types and styling.
