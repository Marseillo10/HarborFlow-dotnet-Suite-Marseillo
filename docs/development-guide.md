# Development Guide

This document provides instructions for setting up the development environment and running the application.

## Prerequisites

-   .NET 9 SDK
-   Node.js and npm (for client-side dependencies)
-   Docker (for running a local PostgreSQL database)

## Environment Setup

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    ```
2.  **Navigate to the project directory:**
    ```bash
    cd HarborFlow_dotnet_Suite_Marseillo_v2
    ```
3.  **Start the PostgreSQL database:**
    A `docker-compose.yml` file is not provided. You will need to manually start a PostgreSQL container.
4.  **Configure the database connection:**
    Update the `DefaultConnection` connection string in `HarborFlowSuite/HarborFlowSuite.Server/appsettings.json` to point to your local PostgreSQL instance.
5.  **Install client-side dependencies:**
    ```bash
    cd HarborFlowSuite/HarborFlowSuite.Client
    npm install
    ```

## Running the Application

1.  **Run the backend:**
    ```bash
    cd HarborFlowSuite/HarborFlowSuite.Server
    dotnet run
    ```
2.  **Run the frontend:**
    ```bash
    cd HarborFlowSuite/HarborFlowSuite.Client
    dotnet run
    ```

## Testing

Run the tests for the different parts of the project using the `dotnet test` command in the respective test project directories.
