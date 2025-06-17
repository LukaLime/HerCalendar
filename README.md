

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
	4. Add Admin account, and User account options
	5. Users are able to subscribe through email, users will get email reminders when an upcoming Cycle is coming up
	6. Update UI using React

Completed Tasks:
	1. Fix Days to next period, and estimated period so it estimates from the last period date using the average cycle length
	1. Add Period Start Date and End Date instead of inputing Cycle Length
	3. Cycle start days, and next period start days should count towards cycle length
	4. When editing, the Next period start date is not needed
	5. When editing, the cycle length can not be changed and is set to 28