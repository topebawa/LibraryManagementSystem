# Library Management System API

A RESTful API for managing library books with JWT-based authentication built with ASP.NET Core.

## Features

- User registration and authentication
- CRUD operations for books
- JWT-based authorization
- Search functionality
- Pagination support
- Swagger documentation
- Entity Framework Core with SQL Server
- Exception handling middleware

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB for development)
- Visual Studio or VS Code

## Getting Started

### 1. Clone the Repository

## terminal
git clone <your-repository-url>
cd LibraryManagementSystem


### 2. Configure the Database

Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance.

### 3. Run the Application

## terminal
dotnet run --project LibraryManagement.API


The API will start at `https://localhost:5001` or `http://localhost:5000`.

### 4. Test the API

- Open Swagger UI at `https://localhost:5001/swagger` to review and test endpoints.
- Register a user via `/api/auth/register`.
- Login via `/api/auth/login` to receive a JWT token.
- Use the JWT token in the `Authorization` header (`Bearer <token>`) for all `/api/books` endpoints.

### 5. Example Requests

**Register:**
POST /api/auth/register
{
  "username": "testuser",
  "password": "Test@123"
}

**Login:**
POST /api/auth/login
{
  "username": "testuser",
  "password": "Test@123"
}

**Authenticated Request:**
Authorization by adding a header: 
Copy the token from the login endpoint 
Go to the Authorize Icon and paste
"Bearer <your-jwt-token>"

Then call any `/api/books` endpoint.



**Note:**  
Sample books (strings) are seeded automatically on the first run.
