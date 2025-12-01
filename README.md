# HarborFlow Suite

## Documentation
For a deep dive into the system architecture, features, and future roadmap (including the Desktop Suite vision), please read the **[Technical Documentation](technical_documentation.md)**.

## Getting Started

### Prerequisites
*   Docker Desktop
*   .NET 9.0 SDK

### Quick Start for Developers
1.  **Start the Environment:**
    ```bash
    ./start-dev.sh
    ```
    This will start the PostgreSQL database in Docker on port **5433**.

2.  **Run the Server:**
    ```bash
    dotnet run --project HarborFlowSuite/HarborFlowSuite.Server --launch-profile https
    ```
    *   **Swagger API Docs**: [https://localhost:7274/swagger](https://localhost:7274/swagger)

3.  **Run the Client:**
    ```bash
    dotnet run --project HarborFlowSuite/HarborFlowSuite.Client
    ```
    *   **Web Application**: [https://localhost:7163](https://localhost:7163)[https://localhost:5205](https://localhost:5205)

### Automatic System Admin Assignment
*   **UGM Accounts**: Any user registering with an email ending in `@mail.ugm.ac.id` will be automatically assigned the **SystemAdmin** role.
*   **Other Accounts**: All other registrations default to the **Guest** role.

### Database Management

*   **Fresh Start:** When you run `start-dev.sh` for the first time, you get a clean database. The application will automatically seed initial data (ports, roles, etc.) on startup.
*   **Sharing Data:**
    *   **Backup:** Run `./backup-data.sh` to create a `harborflow_backup.sql` file containing your current data. Commit this file to share it.
    *   **Restore:** Run `./restore-data.sh` to load the `harborflow_backup.sql` file into your Docker database.
*   **Migrating from Local Legacy DB:**
    *   Run `./migrate-data.sh` to copy data from a local Postgres instance (port 5432) to Docker (port 5433).

### Troubleshooting
*   **Connection Refused:** Ensure you are using `--launch-profile https` for the server.
*   **Database Errors:** Ensure Docker is running and the container `harborflow-db` is up.
