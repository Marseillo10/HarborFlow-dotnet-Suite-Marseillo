# Story 1.2: Real-time Position Updates

Status: drafted

## Story

As a user,
I want vessel positions to update in real-time via SignalR,
so that I always have the most current information.

## Acceptance Criteria

1. Position updates propagate to all connected clients within 1 second.

## Tasks / Subtasks

- [x] Task 1 (AC: #1) - Implement a SignalR hub on the server to broadcast vessel position updates.
  - [x] Subtask 1.1 - Create a `VesselPositionHub` class that inherits from `Hub`.
  - [x] Subtask 1.2 - Add a method to the hub to send position updates to clients.
- [x] Task 2 (AC: #1) - Create a SignalR client service in the Blazor WebAssembly application.
  - [x] Subtask 2.1 - Create a `SignalRService` to manage the connection to the hub.
  - [x] Subtask 2.2 - Implement methods to start and stop the connection.
  - [x] Subtask 2.3 - Expose an event to notify components of new position updates.
- [x] Task 3 (AC: #1) - Connect the map component to the SignalR client to receive and display real-time position updates.
  - [x] Subtask 3.1 - Inject the `SignalRService` into the map component.
  - [x] Subtask 3.2 - Subscribe to the position update event and update the vessel markers on the map.
- [x] Task 4 (AC: #1) - Add a new service to simulate real-time vessel movement and broadcast updates through the SignalR hub.
  - [x] Subtask 4.1 - Create a background service that periodically updates vessel positions.
  - [x] Subtask 4.2 - Inject the `IHubContext<VesselPositionHub>` into the service.
  - [x] Subtask 4.3 - Call the hub method to broadcast the updated positions.
- [ ] Task 5 (AC: #1) - Write unit tests for the SignalR hub and client service.
  - [ ] Subtask 5.1 - Test the hub's message broadcasting.
  - [ ] Subtask 5.2 - Test the client service's connection management and event handling.
- [ ] Task 6 (AC: #1) - Write integration tests to verify real-time updates on the map.
  - [ ] Subtask 6.1 - Create a test that simulates a server-side position update.
  - [ ] Subtask 6.2 - Assert that the map component reflects the updated position within the specified time.

## Dev Notes

- The implementation should use ASP.NET Core SignalR for real-time communication.
- The server-side implementation will be in the `HarborFlowSuite.Server` project.
- The client-side implementation will be in the `HarborFlowSuite.Client` project.
- Consider using a background service for simulating vessel movements for development and testing purposes.

### Project Structure Notes

- `HarborFlowSuite.Server/Hubs/VesselPositionHub.cs`
- `HarborFlowSuite.Client/Services/SignalRService.cs`
- `HarborFlowSuite.Server/Services/VesselPositionUpdateService.cs` (for simulation)

### References

- [Source: docs/epics.md#Epic 1: Real-time Vessel Tracking System]
- [ASP.NET Core SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction)

## Dev Agent Record

### Context Reference

<!-- Path(s) to story context XML will be added here by context workflow -->

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List

### File List
- HarborFlowSuite/HarborFlowSuite.Server/Hubs/VesselPositionHub.cs
- HarborFlowSuite/HarborFlowSuite.Client/Services/VesselPositionSignalRService.cs
- HarborFlowSuite/HarborFlowSuite.Client/Services/IVesselPositionSignalRService.cs
- HarborFlowSuite/HarborFlowSuite.Client/Components/VesselMap.razor
- HarborFlowSuite/HarborFlowSuite.Server/Services/VesselPositionUpdateService.cs
