# Story 1.7: Implement Map Search Highlighting

Status: done

## Story

As a user,
I want to search for vessels by name and see them highlighted on the map,
so that I can easily locate specific vessels.

## Acceptance Criteria

1. Search input allows filtering vessels by name.
2. Selecting a vessel from search highlights it on the map (e.g., zooms to it or changes icon).

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented `MapSearchInput.razor` with `MudAutocomplete`.
- Added highlighting logic in `map.js` (zoom to vessel, open popup).
- Integrated search component into `VesselMap.razor`.

### File List
- HarborFlowSuite/HarborFlowSuite.Client/Components/MapSearchInput.razor
- HarborFlowSuite/HarborFlowSuite.Client/Components/VesselMap.razor
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/map.js
