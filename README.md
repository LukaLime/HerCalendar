

# HerCalendar

## Documentation
 For instructions and details on the development environment, necessary packages to be installed, required dependecies, and how to 
launch the application both locally and on the server, see DEVELOPER.md. For an outline of the structure and design patterns of 
the software, see ARCHITECTURE.md.

## About 
HerCalendar is a web-based menstrual cycle tracking application designed to help users log, view, and predict their menstrual cycles.
Built using ASP.NET Core MVC, Razor Pages, and Entity Framework Core with a SQLite database, the app allows users to input and edit 
period start dates, automatically calculates cycle lengths, and displays a history of previous cycles in a clear table format. 
It also computes the average cycle length and predicts the next period start date, along with the number of days remaining until 
the estimated date. With a clean, responsive interface styled using Bootstrap, HerCalendar offers a simple and effective way for 
users to monitor their reproductive health.


To Do Tasks:
	1. Add Ovulation periods
	2. Add Cycle Chart
	3. Add AI chatbox to analyze the users current tracked cycles, and any other questions the user may have
	4. Users are able to subscribe through email, users will get email reminders when an upcoming Cycle is coming up
	5. Update UI using React
    6. Add an external login provider (Google, Facebook, etc.)
	7. Enforce that UserId is always always points to a valid user in AspNetUsers
	8. Create a smooth transition for every loading page
	9. Add email confirmation when registering a new account
	10. Implement Power BI dashboard for visualizing cycle data and trends
	11. Email news letter for users to subscribe to
	12. Implement donations (Patreon, or Buy Me a Coffee)
	13. Add a blog section
	14. Buy a domain name
	15. Add Google Analytics to track user activity
	16. Add Google AdSense to monetize the website

Completed Tasks:
	1. Fix Days to next period, and estimated period so it estimates from the last period date using the average cycle length
	2. Add Period Start Date and End Date instead of inputing Cycle Length
	3. Cycle start days, and next period start days should count towards cycle length
	4. When editing, the Next period start date is not needed
	5. When editing, the cycle length can not be changed and is set to 28
	6. Set up a Login account authorization for cycle tracker
	7. Each user has there own account with there own cycle tracker
	8. Add Admin account, and User account options
	9. Each user should have there own account with there own cycle tracker
