# Travel and Accommodation Booking Platform
<b>Project Overview</b><br>
This project is a application developed in C#(.NET Core) for FTS company as part of backend developer training.
This project is a comprehensive Travel and Accommodation Booking Platform designed to provide a seamless user experience for booking hotels and accommodations. The platform includes APIs for user authentication, hotel search and details, secure checkout, and admin management functionalities.

## Features

### Core Functionalities
**Login Page**: Secure user authentication with JWT.<br>
**Home Page**: Displays featured hotels and user-specific recommendations.<br>
**Search Results**: Filtered search functionality for hotels.<br>
**Hotel Details**: Detailed hotel information, including amenities and policies.<br>
**Secure Checkout**: Room selection, user information, and payment handling.<br>
**Confirmation Page**: Booking summary and confirmation details.<br>

### Admin Features
**Left Navigation**: Simplified access to admin tools.<br>
**Search Bar**: Search for hotels, rooms, and bookings.<br>
**Detailed Grids**: Display data with sorting and filtering options.<br>
**Create Button**: Add new entities (e.g., hotels, rooms).<br>
**Entity Update Form**: Edit existing data efficiently.<br>

### Technical Requirements
**RESTful APIs**: Built following RESTful principles for scalability and maintainability.<br>
**Authentication**: Secure JWT-based user authentication and authorization.<br>
**Error Handling**: Robust error handling to enhance reliability.<br>
**Validation**: Input validation for all endpoints to ensure data integrity.<br>
**Unit Testing**: Comprehensive test coverage for critical components.<br>
**Documentation**: Swagger/OpenAPI integration for API documentation.<br>

### Project Management
**Tools Used**: Jira for task tracking and progress management.<br>
**Workflow**: Agile development methodology to ensure timely delivery and iterative improvements.<br>

### Technologies
**Backend**: ASP.NET Core Web API<br>
**Database**: SQL Server(SSMS)<br>
**Authentication**: JWT<br>
**Tools**: Postman for testing, Swagger for API documentation<br>
**Other**: Entity Framework Core for database interaction<br>

## Data Storage
This project uses SQL Server Management Studio (SSMS) as the database management system.<br>
The database is built on Microsoft SQL Server (MSSQL), a relational database management system (RDBMS) that supports structured query language (SQL) for managing and querying data.

## Getting Started

### Prerequisites
- .NET SDK 7.0
  
### Installation

1. Clone the repository:
    ```bash
    https://github.com/Wajeed-Mabroukeh/FinalProjcetTariningFTS.git
   ```
2. Make sure to add Server Name and Database Name according to you:
 ```bash
   Server=Server Name;Database=Database Name;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true
 ```

3. Create a Migration:
 ```bash
   dotnet ef migrations add <MigrationName>
 ```

4. Apply the Migration:
   ```bash
   dotnet ef database update
   ```
   
5. Navigate to the project directory:
    ```bash
    cd FinalProjcetTariningFTS
    ```
    
4. Build and run the application:
    ```bash
    dotnet build
    dotnet run
    ```
## Project Structure

```bash
FinalProjcetTariningFTS/
│
├── Program.cs              # Entry point of the application.
├── Models/                 # Contains the data models for Passenger, Booking, Flight, etc.
├── Services/               # Handles business logic (booking, flight search, etc.).
├── Data/                   # File system-based data storage and retrieval.
├── Utilities/              # Helper classes for validation and CSV parsing.
└── README.md               # Project documentation.

