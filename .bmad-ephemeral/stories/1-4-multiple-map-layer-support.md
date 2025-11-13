# Story 1.4: Multiple Map Layer Support

Status: in_progress

## Story

As a user,
I want to switch between different map views (e.g., Street, Satellite)
so that I can customize my map experience.

## Acceptance Criteria

1. Users can switch between Street, Satellite, and other map views.
2. Users can select advanced map layers such as NASA Blue Marble and Sea Surface Temperature.

## Tasks / Subtasks

- [x] Task 1: Identify available map layer providers and their APIs.
  - [x] Subtask 1.1: Research popular map tile providers (e.g., OpenStreetMap, Google Maps, ESRI).
  - [x] Subtask 1.2: Understand their licensing and API usage.
  - [x] Subtask 1.3: Research and implement advanced map layers from NASA GIBS (Blue Marble, Sea Surface Temperature) and add a placeholder for Copernicus.
- [x] Task 2: Implement UI for selecting different map layers.
  - [x] Subtask 2.1: Design and implement a modern floating action button with thumbnail previews for layer selection.
  - [x] Subtask 2.2: Integrate the UI into the map component.
- [x] Task 3: Dynamically switch map layers based on user selection.
  - [x] Subtask 3.1: Implement logic to add/remove map tile layers in Leaflet.
  - [x] Subtask 3.2: Ensure smooth transitions between layers.
- [x] Task 4: Write unit and integration tests for layer switching functionality.

## Dev Notes

- The implementation will extend the existing map component.
- Consider using Leaflet's layer control plugin if available.

### Project Structure Notes

- `HarborFlowSuite/HarborFlowSuite.Client/Components/VesselMap.razor` (modification)
- `HarborFlowSuite/HarborFlowSuite.Client/Components/MapLayerSelector.razor` (new component)

### References

- [Source: docs/epics.md#Epic 1: Real-time Vessel Tracking System]
- [Leaflet.js Documentation](https://leafletjs.com/reference.html)

## Dev Agent Record

### Context Reference

- /Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo-new/.bmad-ephemeral/stories/1-4-multiple-map-layer-support.context.xml

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List

### File List
