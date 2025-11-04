# Community Events API - Quick Start Guide

## 📋 Next Steps

Now that the file structure is created, here are the recommended next steps:

### 1. Restore NuGet Packages
```powershell
cd "d:\Freelancing project\Community event\community-event-ind-api"
dotnet restore
```

### 2. Build the Solution
```powershell
dotnet build
```

### 3. Update Configuration

#### JWT Secret Key
Edit `src/CommunityEventsApi/appsettings.json`:
- Change the `Jwt:Key` to a secure random string (minimum 32 characters)
- Update `Jwt:Issuer` and `Jwt:Audience` if needed

#### Database Connection
Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CommunityEventsDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

For SQL Server:
```json
"DefaultConnection": "Server=localhost;Database=CommunityEventsDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

### 4. Create Database Migrations

```powershell
cd "src\CommunityEventsApi"
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application

```powershell
cd "src\CommunityEventsApi"
dotnet run
```

Or with hot reload:
```powershell
dotnet watch
```

### 6. Test the API

Once running, visit:
- Swagger UI: `https://localhost:5001/swagger` (or check console for actual port)
- API endpoint: `https://localhost:5001/api`

## 🔍 Project Overview

### Key Files to Customize

1. **appsettings.json** - Configuration settings
2. **Program.cs** - Application startup and dependency injection
3. **ApplicationDbContext.cs** - Database context configuration

### Controllers to Implement

The following controllers have basic structure but need implementation:
- `AuthController.cs` - Authentication endpoints
- `UsersController.cs` - User management
- `CommunitiesController.cs` - Community CRUD
- `EventsController.cs` - Event management
- `CommentsController.cs` - Comment system
- `AdminController.cs` - Admin functions

### Services to Complete

Some service methods are marked as `TODO`:
- `EventService.CreateEventAsync()` - Implement event creation with location
- `EventService.RegisterForEventAsync()` - Implement registration logic
- `AuthService.RefreshTokenAsync()` - Implement refresh token mechanism

## 🔧 Common Commands

### Build Commands
```powershell
# Build solution
dotnet build

# Clean build
dotnet clean

# Restore packages
dotnet restore
```

### Database Commands
```powershell
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# List migrations
dotnet ef migrations list
```

### Run Commands
```powershell
# Run application
dotnet run

# Run with watch (hot reload)
dotnet watch

# Run specific project
dotnet run --project src/CommunityEventsApi
```

### Test Commands
```powershell
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## 📦 Installed NuGet Packages

### Main API Project
- `Microsoft.EntityFrameworkCore` - ORM for database access
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Microsoft.EntityFrameworkCore.Tools` - EF Core CLI tools
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
- `AutoMapper` - Object mapping
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI

### Test Project
- `xunit` - Testing framework
- `Moq` - Mocking library
- `FluentAssertions` - Fluent test assertions
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database for testing

## 🎯 Implementation Priority

### Phase 1: Core Functionality
1. ✅ File structure created
2. ⏳ Database migrations
3. ⏳ Authentication implementation
4. ⏳ User management
5. ⏳ Basic CRUD operations

### Phase 2: Event Features
1. ⏳ Event creation and management
2. ⏳ Event registration system
3. ⏳ Geolocation and nearby events
4. ⏳ File upload for images

### Phase 3: Social Features
1. ⏳ Comment system
2. ⏳ Community management
3. ⏳ Event categories

### Phase 4: Advanced Features
1. ⏳ Observability (logging, metrics)
2. ⏳ Unit and integration tests
3. ⏳ API documentation
4. ⏳ Docker deployment

## 🐛 Troubleshooting

### Build Errors
- Make sure .NET 8.0 SDK is installed: `dotnet --version`
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Restore packages: `dotnet restore`

### Database Issues
- Check connection string in appsettings.json
- Ensure SQL Server is running
- For LocalDB: `sqllocaldb start mssqllocaldb`

### Port Already in Use
- Check `Properties/launchSettings.json` for port configuration
- Kill process using the port or change the port number

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [JWT Authentication Guide](https://jwt.io/introduction)
- [AutoMapper Documentation](https://docs.automapper.org)

## 🎉 You're All Set!

Your Community Events API structure is ready. Start by:
1. Running `dotnet restore`
2. Setting up the database
3. Running the application
4. Testing with Swagger UI

Happy coding! 🚀
