# Story 2.1: Firebase Authentication Integration

Status: done

## Description
As a user, I want to be able to register and log in using email/password and social providers via Firebase Authentication.

## Acceptance Criteria
- Users can successfully register and log in with email/password.
- Users can successfully log in with supported social providers.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented Login.razor and Register.razor pages.
- Implemented AuthService.cs to handle Firebase Authentication via JS interop.
- Implemented auth.js and auth-init.js to wrap Firebase SDK.
- Configured Firebase in index.html.

### File List
- HarborFlowSuite/HarborFlowSuite.Client/Pages/Login.razor
- HarborFlowSuite/HarborFlowSuite.Client/Pages/Register.razor
- HarborFlowSuite/HarborFlowSuite.Client/Services/AuthService.cs
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/auth.js
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/auth-init.js
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/index.html
