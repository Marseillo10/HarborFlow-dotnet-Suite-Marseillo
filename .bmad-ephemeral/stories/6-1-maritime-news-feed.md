# Story 6.1: Maritime News Feed

**As a** user,
**I want** to see the latest maritime industry news on the dashboard,
**So that** I can stay informed about relevant developments.

## Acceptance Criteria
- [x] **News Feed Display:** A "Maritime News" section is displayed on the Dashboard (or a dedicated page).
- [x] **RSS Integration:** The system fetches news from a public maritime RSS feed (e.g., Maritime Executive, gCaptain, or similar).
- [x] **Caching:** News feed data is cached for a reasonable duration (e.g., 1 hour) to prevent excessive external requests.
- [x] **UI/UX:** News items are displayed with a title, summary, publication date, and a link to the full article.
- [x] **Responsiveness:** The news feed adapts to different screen sizes.

## Implementation Details
-   **Backend:**
    -   Create `NewsService` to fetch and parse RSS feeds.
    -   Use `IMemoryCache` to cache the parsed feed.
    -   Create an API endpoint `GET /api/news` to return the news items.
-   **Frontend:**
    -   Create `NewsFeed` component.
    -   Add `NewsFeed` to `Dashboard.razor`.
    -   Use `SyndicationFeed` (System.ServiceModel.Syndication) or a lightweight XML parser.
