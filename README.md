# Wajhati

**Wajhati** is a Saudi Arabic web application designed to help users plan and organize trips within Saudi Arabia. It was developed as a practical project during an **ASP.NET Core MVC training program at Tuwaiq Academy**.

The application allows users to explore places, choose a destination city and trip duration, and generate a day-by-day itinerary that combines attractions with restaurants and cafés. Users can then customize the generated plan, add or remove places and days, and save their trips to their accounts for later access.

One of Wajhati's design goals is to help travelers organize their day so that activities and dining options can be located reasonably close to each other, reducing time spent moving between locations.

The project was built using **ASP.NET Core MVC, C#, Entity Framework Core, and SQL Server**, with **ASP.NET Core Identity** for user authentication. A version of the application was also deployed to **Amazon Web Services (AWS)**.

---

## Project Idea

Wajhati was created to simplify domestic trip planning and reduce the time users spend searching for destinations and manually arranging their daily schedules.

Instead of searching for attractions, restaurants, and cafés individually and distributing them across several days, users select a city and the number of days, and the system generates an initial itinerary.

The generated itinerary remains customizable, allowing users to adjust it according to their own trip plans.

---

## Key Features

### Explore Places

Users can browse the places stored in the system and view information such as:

- Name
- City
- Category
- Address
- Description
- Opening hours
- Booking requirements
- Website
- Place image

Each place also has its own details page.

### Search and Filtering

Places can be searched by name and filtered by:

- City
- Place category

Available categories include:

- Entertainment
- Historical places and museums
- Malls
- Workshops and activities
- Restaurants
- Cafés

The current version includes trip support and place data for **Riyadh and Jeddah**.

### Trip Plan Generation

After signing in, users select:

- A destination city
- A trip duration between one and seven days

The system then generates a daily itinerary that typically includes:

- One main destination
- One restaurant
- One café

The application attempts to avoid repeating the same places across different days while alternative options remain available.

In the current implementation, places are selected from the available locations based on city and category. Distance-based itinerary optimization is one of the project's future development goals.

### Trip Customization

Users can customize generated trips by:

- Adding a new day
- Deleting a day
- Editing the contents of a day
- Adding places
- Removing places
- Viewing place details

### Saved Trips

Each user has a **My Trips** section where they can:

- View saved trips
- Filter trips by city
- View complete trip details
- Delete trips

Each trip is associated with the account that created it.

---

## User Authentication

The application uses **ASP.NET Core Identity** for authentication and user management.

It supports:

- User registration
- Login
- Logout
- Persistent authentication sessions
- Associating trips with individual users

---

## Content Management

Wajhati includes a dashboard for managing the content displayed in the application.

It supports:

- Adding places
- Editing place information
- Deleting places
- Adding and editing place details
- Managing opening hours
- Setting booking requirements
- Adding website links
- Uploading place images

Place records include information such as name, category, city, address, latitude, and longitude.

---

## Architecture and Database

The project follows the **Model-View-Controller (MVC)** architecture and uses **Entity Framework Core** with **Microsoft SQL Server**.

The main data relationships can be represented as:

```text
User
  ↓
TripPlan
  ↓
DaySchedule
  ↓
DaySchedulePlace
  ↓
Place
```

A separate relationship is used for extended place information:

```text
Place ↔ PlaceDetail
```

`Place` stores the core information about a location, while `PlaceDetail` stores additional information such as its description, opening hours, booking requirements, image, and website.

The project also uses **EF Core Migrations** to manage the database schema.

---

## Technologies

### Backend
- C#
- ASP.NET Core MVC
- .NET 9
- Entity Framework Core
- ASP.NET Core Identity

### Database
- Microsoft SQL Server
- Entity Framework Core Migrations

### Frontend
- Razor Views
- HTML
- CSS
- JavaScript
- Bootstrap
- Bootstrap Icons
- SweetAlert2

### Cloud & Deployment
- Amazon Web Services (AWS)

---

## Deployment

A version of Wajhati was deployed to **Amazon Web Services (AWS)** during the training program, providing practical experience in moving an ASP.NET Core application from a local development environment to the cloud.

---

## Project Context

Wajhati was developed during an **ASP.NET Core MVC training program at Tuwaiq Academy**.

The project provided hands-on experience with:

- MVC architecture
- CRUD operations
- Relational database design
- Entity Framework Core
- LINQ
- Authentication
- Razor Views
- File and image uploads
- Connecting the frontend, application logic, and database
- Deploying a web application to AWS

---

## Running the Project Locally

### Requirements

- .NET 9 SDK
- Microsoft SQL Server
- Visual Studio or another ASP.NET Core-compatible development environment

After configuring the database connection in `appsettings.json`, apply the migrations and run the application:

```bash
dotnet ef database update
dotnet run
```

---

## Future Improvements

Possible future improvements include:

- Using place coordinates to calculate distances
- Organizing daily itineraries based on nearby locations
- Improving the place-selection algorithm
- Adding interactive maps
- Supporting user interests and preferences
- Expanding support to additional Saudi cities
- Adding place ratings
- Introducing dedicated dashboard roles and permissions
- Adding automated tests for core functionality
