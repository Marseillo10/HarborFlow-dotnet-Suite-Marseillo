# HarborFlow WPF Project TODO List - Phase 3

## Phase 1: Code Quality and Robustness

- [ ] `[backend]` Inject `ILogger` into all services and ViewModels.
- [ ] `[backend]` Add logging for all important events, such as API calls, database operations, and errors.
- [ ] `[backend]` Implement a global exception handler in `App.xaml.cs` to catch unhandled exceptions.
- [ ] `[view]` Display user-friendly error messages for all exceptions.
- [ ] `[backend]` Create validators for `ServiceRequest` and other models.
- [ ] `[view]` Display validation errors in the editor views.

## Phase 2: Feature Enhancements

- [ ] `[backend]` Implement a caching service for offline data storage.
- [ ] `[backend]` Implement a synchronization service to sync offline data.
- [ ] `[view]` Add an online/offline status indicator to the `MainWindow`.
- [ ] `[view]` Add a button to the `MapView` to toggle historical track view.
- [ ] `[backend]` Add a method to `IVesselTrackingService` to get historical vessel positions.
- [ ] `[view]` Implement the historical track display on the map.
- [ ] `[view]` Add a map layer selection UI to the `MapView`.
- [ ] `[view]` Implement vessel marker clustering on the map.
- [ ] `[backend]` Create `IUserProfileService` and `UserProfileService`.
- [ ] `[view]` Create `UserProfileView` and `UserProfileViewModel`.
- [ ] `[view]` Add a "Profile" button to the `MainWindow` to open the `UserProfileView`.

## Phase 3: Development Experience

- [ ] `[devops]` Create a GitHub Actions workflow file (`.github/workflows/dotnet.yml`).
- [ ] `[devops]` Configure the workflow to build and test the application.
- [ ] `[devops]` Add a release step to the workflow.
- [ ] `[devops]` Research and configure SonarQube integration.