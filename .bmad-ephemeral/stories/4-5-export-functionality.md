# Story 4.5: Export Functionality

**As a** Port Authority or Vessel Agent,
**I want** to export data (Service Requests, Vessels) to a CSV file,
**So that** I can perform offline analysis or reporting.

## Acceptance Criteria
- [x] **Service Request Export:** A button on the Service Request Management page allows downloading the list of service requests as a CSV file.
- [x] **Vessel Export:** A button on the Vessel Management page allows downloading the list of vessels as a CSV file.
- [x] **Data Integrity:** The exported file contains all currently filtered/visible data (or all data if no filter).
- [x] **Data Isolation:** The export respects the user's role and company (users only export what they can see).
- [x] **Format:** The output is a valid CSV file with headers.

## Implementation Details
-   **Backend:**
    -   `GET /api/servicerequest/export` endpoint.
    -   `GET /api/vessel/export` endpoint.
    -   Generate CSV content (headers + rows) using `StringBuilder`.
    -   Return `FileResult` with `text/csv` content type.
-   **Frontend:**
    -   Add "Export CSV" button to `ServiceRequestManagement.razor` and `VesselManagement.razor`.
    -   JavaScript interop (or direct link) to trigger the download.
    -   `download.js` created for client-side file saving.
