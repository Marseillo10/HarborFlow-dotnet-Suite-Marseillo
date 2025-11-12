# Architecture: HarborFlowSuite.Client

## 1. Executive Summary

This document outlines the architecture of the `HarborFlowSuite.Client` component, a Blazor WebAssembly application that serves as the primary user interface for the HarborFlow Suite.

## 2. Technology Stack

-   **Framework**: Blazor WebAssembly
-   **Language**: C#
-   **Version**: .NET 9.0
-   **Dependencies**:
    -   Microsoft.AspNetCore.Components.WebAssembly
    -   Microsoft.AspNetCore.SignalR.Client
    -   Blazored.Toast

## 3. Architecture Pattern

The client follows a component-based architecture, with a clear separation of concerns between UI components, services, and providers.

## 4. Data Architecture

The client does not have its own database. It retrieves data from the backend API and caches it in the browser's local storage for offline use.

## 5. API Design

The client communicates with the backend via a RESTful API and a SignalR hub for real-time updates.

## 6. Component Overview

-   **/Pages**: Routable components that represent individual pages in the application.
-   **/Shared**: Reusable UI components.
-   **/Layout**: Components that define the overall layout of the application.

## 7. Development Workflow

Refer to the main `development-guide.md` for instructions on how to set up and run the client application.

## 8. Deployment Architecture

The client is a set of static files that can be deployed to any static web hosting provider. Refer to the main `deployment-guide.md` for more details.

## 9. Testing Strategy

Unit and integration tests for the client are located in the `HarborFlowSuite.Client.Tests` project.
