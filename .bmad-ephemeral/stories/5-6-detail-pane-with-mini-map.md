# Story 5.6: Detail Pane with Mini-Map

## Goal
Enhance the Service Request Detail pane by including a mini-map that shows the current location of the vessel associated with the request. This provides immediate context to the Port Authority Officer without needing to navigate away to the main map.

## Acceptance Criteria
- [x] Create a reusable `VesselMiniMap` component.
- [x] Integrate the `VesselMiniMap` into the `ServiceRequestDetail` component.
- [x] The mini-map should display a marker at the vessel's last known location.
- [x] The mini-map should be centered on the vessel's location.
- [x] If no vessel location is available, handle gracefully (e.g., hide map or show default view).

## Implementation Details
- **Component**: `VesselMiniMap.razor`
- **Integration**: Added to `ServiceRequestDetail.razor` inside the `MudGrid`.
- **Logic**: Fetches vessel positions via `IVesselService` and matches by `VesselId`.
- **Map**: Uses Leaflet via `map.js` with `initMiniMap` and `updateMiniMapMarker` functions.
