<h1>Volunteer Recruiter Platform<h1>
<h2>Overview<h2>
This project provides an online platform to connect volunteers with event organizers. It offers a flexible experience, allowing users to alternate between roles as event recruiters or volunteers based on their activity. Developed with React and Electron, the platform is usable both as a web and desktop application.

<h2>Features<h2>
User Authentication: Simple login and registration process.
Dynamic Roles: Users can create events as recruiters or join events as volunteers.
Event Creation & Management:
    Recruiters can create, edit, and delete events.
    Recruiters can view registered volunteers and send updates.
Volunteer Participation:
    Volunteers can filter, search, join, or leave events.
Email Notifications: Automatic emails notify participants of event changes.

<h2>User Types & Actions<h2>
<h3>Recruiter Actions<h3>
Create Events: Add event details, including dates, location, volunteer type, and participant capacity.
Edit Events: Update event information, automatically notifying registered volunteers.
Delete Events: Remove an event as necessary.
Email Volunteers: Send updates to volunteers about event details.

<h3>Volunteer Actions<h3>
Search Events: Use filters to find relevant events based on location, date, or volunteer type.
Join Events: Register for events with a single click.
Cancel Participation: Withdraw from events as needed.


<h2>Main Processes<h2>
Event Creation (Recruiter): Adds a new event, specifying details like location, type, dates, and capacity.
Event Search & Filter (Volunteer): Filters events to find suitable opportunities based on personal preferences.
Event Updates (Recruiter): Sends automatic email notifications to volunteers when event details change.
Event Registration (Volunteer): Registers for an event, updating participant counts automatically.
Volunteer List View (Recruiter): Displays a list of registered volunteers for an event.

<h2>Installation<h2>
Prerequisites
Node.js and npm installed

<h2>Install Dependencies<h2>
npm install

<h2>Running the Application<h2>
Development Mode
Web App: Starts the web application.
npm start