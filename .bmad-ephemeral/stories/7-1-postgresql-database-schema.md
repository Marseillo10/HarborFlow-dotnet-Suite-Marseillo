# Story 7.1: PostgreSQL Database Schema

**As a** developer,
**I want** to migrate the application to use a robust PostgreSQL database,
**So that** I can ensure data persistence, integrity, and scalability for the HarborFlow Suite.

## Acceptance Criteria
- [x] **Docker Integration:** A `docker-compose.yml` file is created to spin up a PostgreSQL container.
- [x] **Configuration:** The application (`appsettings.json`) is configured to connect to the PostgreSQL instance.
- [x] **Migration Scripts:** Scripts (`start-dev.sh`, `migrate-data.sh`) are created to facilitate easy startup and data migration.
- [x] **Schema Verification:** The database schema correctly reflects the Entity Framework Core models (Users, Roles, Vessels, ServiceRequests).
- [x] **Documentation:** Instructions for running the database are added to the README or Technical Documentation.

## Implementation Details
-   **Infrastructure:**
    -   Create `docker-compose.yml` with `postgres:15` image.
    -   Configure volume mapping for data persistence.
-   **Backend:**
    -   Update `ApplicationDbContext` to use `Npgsql`.
    -   Ensure connection strings are properly managed.
-   **DevOps:**
    -   Create shell scripts for developer convenience.
