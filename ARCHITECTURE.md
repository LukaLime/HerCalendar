# Architecture

This document outlines the architecture for a web application that follows the Model-View-Controller (MVC) design pattern, using JavaScript, HTML/CSS, Node.js, and SQLite.

## Overview

The application consists of three main components: the Model, the View, and the Controller.

- The Model represents the data and business logic of the application.
- The View displays the data and provides the user interface.
- The Controller handles user input, updates the Model, and updates the View accordingly.

## File Structure
```
|   Program.cs
|   Connected 
|   package.json
|   hercalendar.db
|   appsettings.json
|   appsettings.Development.json
|   ARCHITECTURE.md
|   DEVELOPER.md
|   package.json
|   README.md
|
+---Model
|   CycleTracker.cs
|   ErrorViewModel.cs
|
+---Data
|   ApplicationDbContext.cs
|   +---Migrations
|   |       20240422123456_InitialCreate.cs
|   |       ApplicationDbContextModelSnapshot.cs
|
+---Views
|   +---Home
|   |       Index.cshtml
|   |       Privacy.cshtml
|   +---MyCycles
|   |       Create.cshtml
|   |       Delete.cshtml
|   |       Details.cshtml
|   |       Edit.cshtml
|   |       Index.cshtml
|   +---Shared
|   |       _Layout.cshtml
|   |       _LoginPartial.cshtml
|	_ViewImports.cshtml
|	_ViewStart.cshtml
+---Controllers
|   HomeController.cs
|   MyCyclesController.cs
|   CyclesApiController.cs
+---wwwroot
	+---css
	|       site.css
	|       bootstrap.min.css
	+---js
	|       site.js
	|       bootstrap.bundle.min.js
	+---lib
			jquery
			bootstrap
			fontawesome-free
+---ClientApp (if using React or other frontend framework)
+---Areas
	+---Identity (if using ASP.NET Identity for authentication)

```

## Model

The Model represents the core data and business logic of the HerCalendar application. It manages menstrual cycle data, including start dates and calculated cycle lengths.
Models also handle validation, such as ensuring the next period date is after the last period date.

This application uses SQLite as the database engine, which is lightweight and easy to set up. Tools like SQLiteStudio can be used to view, modify, and query the data.

For HerCalendar, the primary model is the CycleTracker, which stores information like:

 -> LastPeriodStartDate

 -> NextPeriodStartDate

 -> CycleLength

This model interacts with the database using Entity Framework Core (EF Core) for ORM (Object-Relational Mapping), allowing you to perform queries and updates using C# 
classes instead of raw SQL.

## View

The View is responsible for displaying information to the user and capturing user input in a clean, modern, and mobile-responsive interface.

In HerCalendar, the views are implemented using Razor (.cshtml) files with embedded C# logic. They use:

 -> HTML for structure
 -> CSS for styling (with Bootstrap for responsiveness)
 -> JavaScript for interactivity

## Controller

The Controller handles user input, processes it, updates the model, and returns the appropriate view or data. It acts as the bridge between the model and the view.

HerCalendar uses ASP.NET MVC Controllers. For example:

 -> HomeController: Manages general pages like the home page and privacy policy.
 -> MyCyclesController: Handles CRUD operations for menstrual cycle data.
 -> CyclesApiController: Provides API endpoints for AJAX requests or external integrations.

Controllers receive user actions, validate and process data (e.g., calculate cycle length based on dates), update the database, and then return a view or API response.

## Conclusion

HerCalendar follows the MVC (Model-View-Controller) design pattern provided by ASP.NET Core. This separation of concerns makes the application:
 -> Maintainable: Each component has a clear responsibility.
 -> Scalable: New features can be added with minimal impact on existing code.
 -> Testable: Individual components can be tested in isolation.

 By using ASP.NET Core MVC, Entity Framework, and SQLite, HerCalendar is structured in a way that is scalable, user-friendly, and maintainable for future enhancements
 such as user authentication, cycle predictions, or personalized insights.