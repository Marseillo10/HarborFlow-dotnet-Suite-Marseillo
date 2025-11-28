# Story 2.4: User Profile Management

Status: done

## Description
As a user, I want to view and update my basic profile information.

## Acceptance Criteria
- Users can view their profile information.
- Users can update basic profile information.
- Users can change their password with re-authentication if required.

## Dev Agent Record

### Context Reference

### Agent Model Used

{{agent_model_name_version}}

### Debug Log References

### Completion Notes List
- Implemented UserProfileController to handle GET and PUT requests for user profiles.
- Implemented UserProfile.razor to display and edit profile data.
- Implemented UserProfileService (implied) to handle business logic.
- Implemented Change Password feature in `UserProfile.razor` using Firebase `updatePassword`.
- Added `reauthenticate` function in `auth.js` and `ReauthDialog.razor` to handle `requires-recent-login` errors seamlessly.

### File List
- HarborFlowSuite/HarborFlowSuite.Server/Controllers/UserProfileController.cs
- HarborFlowSuite/HarborFlowSuite.Client/Pages/UserProfile.razor
- HarborFlowSuite/HarborFlowSuite.Client/wwwroot/js/auth.js
- HarborFlowSuite/HarborFlowSuite.Client/Components/ReauthDialog.razor
