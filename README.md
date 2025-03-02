# Medication Management API Documentation

## Overview

This API allows users to manage their medications and user authentication securely. The API provides endpoints for:

- Registering a new medication
- Retrieving medications
- Updating medication details
- Deleting medications
- User authentication (SignUp & Login)

## Prerequisites

Before setting up the project, ensure you have the following installed:

- .NET SDK 6 or later
- SQL Server or any configured database
- Entity Framework Core
- Postman (optional, for API testing)
- Git (for version control)

## Installation Steps

1. **Clone the Repository:**
   ```sh
   git clone "https://github.com/akankshya16/ehrsafenew.git"
  
   ```
2. **Configure the Database:**
CREATE DATABASE EHR_DB;
GO
USE EHR_DB;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-incremented primary key
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Address NVARCHAR(255),
    City NVARCHAR(100),
    Country NVARCHAR(100),
    Postcode NVARCHAR(20),
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(255) UNIQUE,
    Password NVARCHAR(255)
);

CREATE TABLE Medications (
    Id INT IDENTITY(1,1) PRIMARY KEY,  
    UserId INT NOT NULL,  
    Description NVARCHAR(255) NOT NULL,
    Dosage NVARCHAR(50) NOT NULL,
    Frequency NVARCHAR(50) NOT NULL,
    Duration INT NOT NULL, 
    Reason NVARCHAR(255),
    DateOfIssue DATE NOT NULL,
    Instructions NVARCHAR(500),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

 "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EHR_DB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}



3. **Run the Application:**
   ```sh
   dotnet run
   ```

## API Endpoints

### Authentication Endpoints

#### Sign Up

**Endpoint:** `POST /api/token/signup`

- Registers a new user.
- Requires email and password in the request body.

#### Login

**Endpoint:** `GET /api/token/login`

- Authenticates a user and returns a JWT token.
- Requires `email` and `password` as query parameters.

### Medication Management Endpoints

#### Register Medication

**Endpoint:** `POST /api/medication/register`

- Adds a new medication record for the logged-in user.
- Requires medication details in the request body.

#### Get Medications

**Endpoint:** `GET /api/medication/get`

- Retrieves a list of medications for the logged-in user.
- Supports filtering by `afterDateOfIssue`, `description`, `frequency`, and `reason`.

#### Update Medication

**Endpoint:** `PUT /api/medication/update/{id}`

- Updates an existing medication.
- Requires medication details in the request body.

#### Delete Medication

**Endpoint:** `DELETE /api/medication/delete/{id}`

- Deletes a specific medication record.

## Security Considerations

- All API endpoints require authentication except `signup` and `login`.
- JWT tokens must be included in the `Authorization` header for protected endpoints.

## .gitignore Configuration

To prevent sensitive files from being committed, ensure your `.gitignore` includes:

```gitignore
bin/
obj/
appsettings.json
appsettings.Development.json
secrets.json
.env
```

## Testing the API

You can test the API using **Postman** or any API testing tool:

1. Obtain a JWT token from the `login` endpoint.
2. Include the token in the `Authorization` header as `Bearer <token>` for protected endpoints.

---

