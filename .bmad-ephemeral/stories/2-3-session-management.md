# Story 2.3: Session Management

Status: done

## Description
As a user, I want my session to persist across browser sessions with automatic token refresh for a seamless experience.

## Acceptance Criteria
- User sessions persist across browser sessions.
- Tokens are automatically refreshed.
- Users are automatically logged out after 15 minutes of inactivity.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented FirebaseAuthenticationStateProvider to manage auth state in Blazor.
- Initialized auth listener in App.razor via JS interop.
- Implemented OnAuthStateChanged handler in auth-init.js to sync Firebase state with Blazor.
- Implemented Idle Timeout feature using `idle-timer.js` and `IdleTimeoutService`.
- Configured automatic logout after 15 minutes of inactivity in `MainLayout.razor`.

### File List
- HarborFlowSuite/HarborFlowSuite.Client/App.razor
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/auth-init.js
- HarborFlowSuite/HarborFlowSuite.Client/Providers/FirebaseAuthenticationStateProvider.cs
- HarborFlowSuite/HarborFlowSuite.Client/Services/IdleTimeoutService.cs
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/idle-timer.js
- HarborFlowSuite/HarborFlowSuite.Client/Layout/MainLayout.razor
