# Test Log (TEST.md)

This file logs the test plan and results for the implementation.

---

## Test Plan: Live Data Verification (aisstream.io)

**Objective:** To verify that the application correctly connects to the `aisstream.io` WebSocket, receives streaming data, and displays live vessel positions on the map.

**Prerequisites:** The user's `aisstream.io` API key is already in `appsettings.json`.

**Test Steps:**

1.  **Run the Application:**
    *   Execute `dotnet run --project HarborFlow.Wpf`.

2.  **Perform In-App Test:**
    *   User logs in to the application.
    *   User navigates to the map view.
    *   User waits for 30-60 seconds for the WebSocket to connect and data to stream.

3.  **Observe and Report Outcome:**
    *   User will answer the following questions:
        *   **[Question 1]** Does the map display vessel markers? Do new markers appear over time?
        *   **[Question 2]** Do the vessels appear in realistic, real-world locations within the defined bounding box (Southeast Asia)?
        *   **[Question 3]** Did you see any error notifications inside the application?
        *   **[Question 4]** Please check the console output. Do you see a log entry stating `"WebSocket connection established."` followed by `"Subscription message sent."`? If you see any errors logged, please provide them.

---