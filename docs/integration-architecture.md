# Integration Architecture

This document describes how the different parts of the HarborFlow Suite application communicate with each other.

## Communication Patterns

The application follows a client-server architecture where the `HarborFlowSuite.Client` (a Blazor WebAssembly application) communicates with the `HarborFlowSuite.Server` (an ASP.NET Core Web API) via HTTP requests and SignalR for real-time communication.

The backend itself is composed of several library projects (`HarborFlowSuite.Application`, `HarborFlowSuite.Core`, and `HarborFlowSuite.Infrastructure`) that are used by the `HarborFlowSuite.Server` project.

## Integration Points

| From                     | To                       | Type      | Details                                      |
| ------------------------ | ------------------------ | --------- | -------------------------------------------- |
| HarborFlowSuite.Client   | HarborFlowSuite.Server   | HTTP      | RESTful API calls for data retrieval and manipulation. |
| HarborFlowSuite.Client   | HarborFlowSuite.Server   | SignalR   | Real-time updates for vessel positions and notifications. |
| HarborFlowSuite.Server   | HarborFlowSuite.Application | Library   | Uses application service interfaces.         |
| HarborFlowSuite.Server   | HarborFlowSuite.Core     | Library   | Uses core domain models and DTOs.            |
| HarborFlowSuite.Server   | HarborFlowSuite.Infrastructure | Library   | Uses infrastructure services for data persistence. |
| HarborFlowSuite.Application | HarborFlowSuite.Core     | Library   | Uses core domain models and DTOs.            |
| HarborFlowSuite.Infrastructure | HarborFlowSuite.Application | Library   | Implements application service interfaces.   |
| HarborFlowSuite.Infrastructure | HarborFlowSuite.Core     | Library   | Uses core domain models.                     |
