# HarborFlow

HarborFlow is a Smart Port Management System designed to digitize and streamline operational workflows in a port. This project is built as a desktop application using .NET 9 with Windows Presentation Foundation (WPF), following the MVVM pattern.

## Features

- **Real-time Vessel Tracking:** Track vessel positions in real-time on an interactive map using AIS data.
- **Digital Workflow Management:** A platform for submitting and approving port service requests.
- **Centralized Information Portal:** A single point of access for all port-related information for stakeholders.

## Technology Stack

- **.NET 9 WPF:** For the desktop application.
- **PostgreSQL:** As the primary database.
- **Entity Framework Core 9:** For data access.
- **MVVM Pattern:** For a clean and maintainable architecture.

## Project Structure

The solution is divided into the following projects, following the principles of Clean Architecture:

- **HarborFlow.Core:** This is the core of the application, containing the domain models (entities) and the interfaces for the services. It has no dependencies on other projects in the solution.
- **HarborFlow.Application:** This project contains the business logic of the application. It implements the interfaces defined in `HarborFlow.Core` and orchestrates the domain models to perform application-specific tasks.
- **HarborFlow.Infrastructure:** This project handles all external concerns, such as data access, file system access, and communication with external APIs. It implements the interfaces defined in `HarborFlow.Core` and provides the concrete implementation for the data access layer using Entity Framework Core.
- **HarborFlow.Wpf:** This is the presentation layer of the application, built with WPF and the MVVM pattern. It is responsible for the user interface and user experience.
- **HarborFlow.Tests:** This project contains unit and integration tests for the application, ensuring the quality and correctness of the code.

## How to Run

1.  **Clone the repository.**
2.  **Configure the database connection:**
    - Open the `appsettings.json` file in the `HarborFlow.Wpf` project.
    - Update the `DefaultConnection` string with your PostgreSQL connection details.
3.  **Run the database migrations:**
    - Open a terminal in the `HarborFlow.Infrastructure` directory.
    - Run the command: `dotnet ef database update`
4.  **Run the application:**
    - Set the `HarborFlow.Wpf` project as the startup project.
    - Run the application from Visual Studio or by using the command `dotnet run` in the `HarborFlow.Wpf` directory.
