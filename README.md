# Library Martha API

## Overview
Library Martha is a robust, RESTful API built to manage a library system. The project adopts **Clean Architecture**, ensuring scalability and maintainability, and leverages the **CQRS** pattern with MediatR for organized request handling. It also integrates third-party services like **SendGrid** for email notifications to enhance user engagement.

## Features
- **Clean Architecture**: Implements a layered design separating application concerns.
- **CQRS Pattern with MediatR**: Streamlines request handling for commands and queries.
- **Unit and Integration Testing**: Ensures high reliability and maintainability.
- **Book Management**: Full CRUD operations for managing books in the library.
- **Reservation System**: Allows users to reserve books for borrowing.
- **User Authentication and Authorization**: Secured with ASP.NET Core Identity and JWT tokens.
- **Email Notifications**: Sends real-time updates using **SendGrid**.
- **Swagger/OpenAPI Documentation**: Interactive API documentation for easier testing.
- **Background Processing**: Automates reservation checks and updates, updates book availabilty and sends notification based on book availability.

## Technologies Used
- **ASP.NET Core**: Backend framework.
- **Entity Framework Core**: Database ORM.
- **JWT Authentication**: Secure token-based authentication.
- **MediatR**: Implements the CQRS pattern.
- **SendGrid**: Third-party email notification service.
- **XUnit**: For unit and integration testing.
- **FluentValidation**: Validates input data.
- **Swagger**: Generates interactive API documentation.
- **Dependency Injection**: Built-in DI container for service management.

## Setup Instructions
1. **Clone Repository**: 
   ```bash
   git clone https://github.com/YourUsername/LibraryMartha.git
   cd LibraryMartha
   ```
2. **Configure Settings**:
   - Update `appsettings.json` with:
     - **Database Connection String**.
     - **JWT Secret Key** under `JwtSettings`.
     - **SendGrid API Key** under `EmailSettings`.

3. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Build and Run**:
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**:
   Navigate to `https://localhost:{PORT}/swagger/index.html` to explore and test the API.

## Usage
- Use **Swagger** to test endpoints.
- Authenticate using a JWT token for protected routes.
- Manage books and reservations via the API.
- Receive email notifications for reservation updates.

## Testing
- **Run Unit Tests**:
  ```bash
  dotnet test
  ```
- Includes tests for:
  - **Controllers**
  - **Commands & Queries**
  - **Background Services**
  - **Email Integration**

## Contributors
- [Thabani Ngwenya](https://github.com/Patiant-X)

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
