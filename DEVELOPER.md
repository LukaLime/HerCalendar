# Developer Guide – HerCalendar

## Software Used

This project is built using ASP.NET Core MVC with C# and Razor Pages for the frontend. 
The database is implemented using SQLite and managed via Entity Framework Core (EF Core), 
which allows object-relational mapping without writing raw SQL. Styling is handled using Bootstrap (will be updated to React in the future).

## Prerequisites

### Tools

 Make sure you have the following tools installed before starting:

 -> ASP.NET SDK (for framework and libraries)
 -> Visual Studio or VS Code
 -> Azure Cloud DB (for database)
 -> SQLiteStudio (for database inspection)
 -> npm (for managing JavaScript packages)

## Running the Application Locally

 1. Clone the project repository.
 2. Open the project in Visual Studio or VS Code.
 3. Build the solution using the terminal or IDE.


## ASP.NET Core Application Responsibilities

### Controllers

 Controllers handle HTTP requests, process data using the Model, and return Views.

### Views

 Views are .cshtml files using Razor syntax for rendering dynamic HTML based on model data.

### Models

 Models represent the data structure of the application (e.g., CycleTracker). EF Core maps these models to the SQLite database, enabling you to query and update data via C#.

## SQLite Database
 The application uses SQLite for data storage.

 -> The database file (e.g., HerCalendar.db) is located in the project root or Data folder.
 -> Data is accessed and manipulated using EF Core’s DbContext (ApplicationDbContext.cs).
 -> Use SQLiteStudio or similar tools to view and manage data manually.

## Entity Framework Core (EF Core)

 EF Core is used for data access and migration management. You can use these commands to manage the database:

 -> dotnet ef migrations add <MigrationName> – Create a migration
 -> dotnet ef database update – Apply migrations to update the database

 ## ERD Diagram

 +---------------------+           +---------------------------+
|     AspNetUsers     |<--------->|      CycleTracker         |
|---------------------|    1   *  |---------------------------|
| Id (PK)             |-----------| Id (PK)                   |
| UserName            |           | CycleLength               |
| Email               |           | LastPeriodStartDate       |
| EmailConfirmed      |           | NextPeriodStartDate       |
| ...                 |           | UserId (FK -> AspNetUsers)|
+---------------------+           +---------------------------+

                ↑
                |
                | Role-based access control
                |
        +---------------------+
        |   AspNetRoles       |
        |---------------------|
        | Id (PK)             |
        | Name                |
        +---------------------+


🧩 Tables Involved
🔹 AspNetUsers (from ASP.NET Core Identity)
Primary Key: Id (string)

Other fields: Email, UserName, EmailConfirmed, etc.

🔹 CycleTracker
Primary Key: Id (int)

Foreign Key: UserId → AspNetUsers.Id

Fields:

CycleLength (int)

LastPeriodStartDate (DateTime)

NextPeriodStartDate (DateTime)

🔹 AspNetRoles / AspNetUserRoles (handled by Identity)
You use the "Admin" role via [Authorize(Roles = "Admin")]

## Contributing

 To contribute to the project:

 1. Fork the project repository.
 2. Create a new branch for your changes.
 3. Make your changes and test them.
 4. Commit with clear messages.
 5. Push to your forked repository.
 6. Create a pull request.

## Useful Links

 1. ASP.NET Core Docs: https://learn.microsoft.com/en-us/aspnet/core/
 2. EF Core Docs: https://learn.microsoft.com/en-us/ef/core/
 3. Bootstrap Docs: https://getbootstrap.com/

