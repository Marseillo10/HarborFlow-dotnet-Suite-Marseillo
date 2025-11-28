# Story 1.8: Handle Live Feed Interruption State

Status: done

## Story

As a user,
I want to be clearly informed when the live data feed is interrupted so I don't act on stale data.

## Acceptance Criteria

1. A non-intrusive banner appears when the data feed is down.
2. Vessel icons change appearance (e.g., turn grey) to indicate their data is not live.

## Tasks / Subtasks

- [x] Task 1: Implement logic to detect SignalR connection loss/interruption.
- [x] Task 2: Create a non-intrusive banner component to display connection status.
- [x] Task 3: Update `VesselMap.razor` and `map.js` to visually indicate stale data (grey out icons).
- [x] Task 4: Verify interruption handling (simulate connection loss).

## Dev Notes

- Use SignalR client events (`OnClosed`, `OnReconnecting`, `OnReconnected`) to detect state.
- The banner should probably be a `MudAlert` or similar at the top of the map or dashboard.
- For vessel icons, we can add a CSS class (e.g., `vessel-stale`) that applies a grayscale filter.
