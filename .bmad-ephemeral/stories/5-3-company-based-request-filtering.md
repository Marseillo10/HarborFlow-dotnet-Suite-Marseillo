# Story 5.3: Company-Based Request Filtering

**As a** user,
**I want** to see only service requests relevant to my company/role,
**So that** I can focus on my specific responsibilities and ensure data privacy.

## Acceptance Criteria
- [x] **Role-Based Filtering:**
    -   **System Admin:** Can see ALL service requests.
    -   **Port Authority Officer:** Can see ALL service requests (to manage port operations).
    -   **Vessel Agent:** Can ONLY see service requests associated with their own company.
    -   **Guest:** Cannot see any service requests.
- [x] **Visual Confirmation:** The list of service requests automatically filters based on the logged-in user's role and company.
- [x] **Security:** API endpoints enforce this filtering to prevent unauthorized access via direct API calls.

## Implementation Details
-   **Backend:**
    -   Update `ServiceRequestService.GetAllServiceRequestsAsync` (or equivalent) to accept the current user's ID/Role/CompanyId.
    -   Implement filtering logic in the query:
        -   If Admin/PortAuthority -> Return all.
        -   If VesselAgent -> Return `Where(r => r.CompanyId == user.CompanyId)`.
-   **Frontend:**
    -   Ensure the `ServiceRequestManagement` page passes the necessary context or relies on the API to return the filtered list.
    -   (Optional) Display a visual indicator of the current filter scope (e.g., "Showing all requests" vs "Showing requests for [Company Name]").

## Dependencies
-   Story 5.1 (Completed)
-   Story 3.3 (Company-Based Data Isolation - In Progress/Done?)
