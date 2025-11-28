# Story 1.6: Implement Vessel Hover Tooltip

Status: done

## Story

As a user,
I want to see a summary of vessel information when I hover over a vessel icon,
so that I can quickly identify vessels without clicking.

## Acceptance Criteria

1. Hovering over a vessel icon displays a tooltip with vessel name and type.
2. Tooltip disappears when mouse leaves the icon.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented `VesselTooltip.razor` component.
- Added hover event handling in `map.js` (`mouseover`, `mouseout`).
- Integrated tooltip into `VesselMap.razor`.

### File List
- HarborFlowSuite/HarborFlowSuite.Client/Components/VesselTooltip.razor
- HarborFlowSuite/HarborFlowSuite.Client/Components/VesselMap.razor
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/map.js
