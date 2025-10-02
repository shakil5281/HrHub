# HR Hub API

A comprehensive RESTful API built with C# .NET 10 featuring role-based authentication and authorization for HR management systems.

## Features

- **JWT Authentication & Authorization**: Secure token-based authentication with role-based access control
- **Role-Based Access Control**: Three predefined roles (Admin, Manager, Employee) with different permission levels
- **Entity Framework Core**: Database operations with SQL Server integration
- **OpenAPI Documentation**: Built-in API documentation and testing interface
- **CORS Support**: Cross-origin resource sharing enabled for frontend integration
- **Comprehensive Logging**: Structured logging throughout the application

## Technology Stack

- **.NET 10 (Preview)**: Latest .NET framework
- **ASP.NET Core Web API**: RESTful API framework
- **Entity Framework Core**: Object-relational mapping
- **SQL Server**: Database management system
- **JWT (JSON Web Tokens)**: Authentication mechanism
- **ASP.NET Core Identity**: User management system
- **OpenAPI/Swagger**: API documentation

## Database Configuration

The API is configured to connect to SQL Server with the following settings:
- **Server**: `SHAKIL\\SQLEXPRESS`
- **Database**: `HrHubDB`
- **Username**: `sa`
- **Password**: `12345`
- **Connection String**: `Server=SHAKIL\\SQLEXPRESS;Database=HrHubDB;User Id=sa;Password=12345;TrustServerCertificate=true;`

## Default Roles

The system comes with three predefined roles:

1. **Admin**: Full system access including user management and system configuration
2. **Manager**: Limited administrative access for employee management within departments
3. **Employee**: Basic access for personal information and standard operations

## Default Admin User

A default administrator account is automatically created:
- **Email**: `admin@hrhub.com`
- **Password**: `Admin@123`
- **Role**: Admin

## API Endpoints

### Authentication Endpoints (`/api/auth`)

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - User login
- `POST /api/auth/assign-role` - Assign role to user (Admin only)
- `GET /api/auth/profile` - Get current user profile

### Employee Management (`/api/employee`)

- `GET /api/employee` - Get all employees (Admin/Manager only)
- `GET /api/employee/{id}` - Get employee by ID (Admin/Manager only)
- `GET /api/employee/department/{department}` - Get employees by department (Admin/Manager only)
- `PUT /api/employee/{id}` - Update employee information (Admin only)
- `DELETE /api/employee/{id}` - Deactivate employee (Admin only)

### Dashboard Endpoints (`/api/dashboard`)

- `GET /api/dashboard/employee` - Employee dashboard (All authenticated users)
- `GET /api/dashboard/manager` - Manager dashboard (Manager/Admin only)
- `GET /api/dashboard/admin` - Admin dashboard (Admin only)
- `GET /api/dashboard/stats` - System statistics (Admin only)

## Getting Started

### Prerequisites

- .NET 10 SDK (Preview)
- SQL Server (Express or full version)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository** (if applicable)
   ```bash
   git clone <repository-url>
   cd HrHubAPI
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (if needed)
   - Edit `appsettings.json` to modify the connection string if your SQL Server configuration is different

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTP: `http://localhost:5049`
- OpenAPI Documentation: `http://localhost:5049/openapi/v1.json`

## Usage Examples

### 1. User Registration

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "john.doe@company.com",
  "password": "SecurePass123",
  "confirmPassword": "SecurePass123",
  "firstName": "John",
  "lastName": "Doe",
  "department": "IT",
  "position": "Software Developer"
}
```

### 2. User Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@hrhub.com",
  "password": "Admin@123"
}
```

Response:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiration": "2025-10-01T15:15:07Z",
    "user": {
      "id": "user-id",
      "email": "admin@hrhub.com",
      "firstName": "System",
      "lastName": "Administrator",
      "department": "IT",
      "position": "System Administrator",
      "roles": ["Admin"]
    }
  }
}
```

### 3. Accessing Protected Endpoints

Include the JWT token in the Authorization header:

```http
GET /api/dashboard/admin
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Role-Based Access Examples

### Admin Access
- Can access all endpoints
- Can manage users and assign roles
- Can view system statistics and admin dashboard

### Manager Access
- Can view employee lists and details
- Can access manager dashboard
- Cannot modify system settings or assign roles

### Employee Access
- Can view own profile
- Can access employee dashboard
- Cannot access other users' information or administrative functions

## Security Features

- **JWT Token Authentication**: Secure stateless authentication
- **Password Requirements**: Strong password policies enforced
- **Role-Based Authorization**: Fine-grained access control
- **CORS Protection**: Configurable cross-origin policies
- **SQL Injection Protection**: Parameterized queries via Entity Framework
- **Input Validation**: Comprehensive model validation

## Error Handling

The API returns consistent error responses:

```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error messages"]
}
```

Common HTTP status codes:
- `200 OK`: Successful operation
- `400 Bad Request`: Invalid input data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Configuration

### JWT Settings (`appsettings.json`)

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast256BitsLongForHS256Algorithm",
    "Issuer": "HrHubAPI",
    "Audience": "HrHubAPI",
    "ExpiryMinutes": 60
  }
}
```

### Database Connection

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SHAKIL\\SQLEXPRESS;Database=HrHubDB;User Id=sa;Password=12345;TrustServerCertificate=true;"
  }
}
```

## Development

### Project Structure

```
HrHubAPI/
├── Controllers/           # API controllers
│   ├── AuthController.cs
│   ├── EmployeeController.cs
│   └── DashboardController.cs
├── Data/                  # Database context
│   └── ApplicationDbContext.cs
├── DTOs/                  # Data transfer objects
│   └── AuthenticationDTOs.cs
├── Models/                # Entity models
│   ├── ApplicationUser.cs
│   └── ApplicationRole.cs
├── Services/              # Business logic services
│   ├── IJwtService.cs
│   └── JwtService.cs
├── Program.cs             # Application entry point
└── appsettings.json       # Configuration settings
```

### Adding New Endpoints

1. Create a new controller in the `Controllers` folder
2. Apply appropriate authorization attributes (`[Authorize]`, `[Authorize(Roles = "Admin")]`)
3. Implement error handling and logging
4. Return consistent API responses using `ApiResponse<T>`

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in `appsettings.json`
   - Ensure database user has proper permissions

2. **JWT Token Issues**
   - Verify token is included in Authorization header
   - Check token expiration
   - Ensure secret key is properly configured

3. **Role Authorization Issues**
   - Verify user has required role assigned
   - Check role names are spelled correctly
   - Ensure user is properly authenticated

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions, please contact the development team or create an issue in the repository.
