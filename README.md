# Community Events API

A community-driven events platform where community leaders/hosts can create events (with photos, description, registration), members join events with their community ID, comment and ask questions, search events by location/type, and where admins moderate and close events after completion.

##  Quick Start

### Run the Application
```powershell
# From project root
dotnet run --project src\CommunityEventsApi\CommunityEventsApi.csproj
```

### Access Swagger API Documentation
Once running, visit: **https://localhost:50126/swagger**

The application will show you the exact URLs in the console output.

##  Essential Commands

### Development
```powershell
# Run with hot reload (recommended)
dotnet watch --project src\CommunityEventsApi\CommunityEventsApi.csproj

# Build project
dotnet build

# Run tests
dotnet test
```

### Database Setup (First Time)
```powershell
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate --project src\CommunityEventsApi

# Update database
dotnet ef database update --project src\CommunityEventsApi
```

##  API Endpoints Summary

### Authentication
- **POST** `/api/auth/signup` - Register user
- **POST** `/api/auth/login` - User login

### Events
- **GET** `/api/events` - List all events
- **POST** `/api/events` - Create event
- **POST** `/api/events/{id}/register` - Register for event

### Communities  
- **GET** `/api/communities` - List communities
- **POST** `/api/communities` - Create community

See full documentation in Swagger UI: **https://localhost:PORT/swagger**

##  Tech Stack

- .NET 8.0
- Entity Framework Core + SQL Server
- JWT Authentication
- Swagger/OpenAPI
- AutoMapper

##  Project Structure

```
src/CommunityEventsApi/
 Controllers/     # API endpoints
 Services/        # Business logic (BAL)
 Repositories/    # Data access (DAL)
 Models/          # Database entities
 DTOs/            # Request/Response objects
 Middleware/      # Auth, logging, exceptions
 Helpers/         # Utilities (password, token, geo)
```

##  Configuration

Edit `src/CommunityEventsApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CommunityEventsDb;Trusted_Connection=true"
  },
  "Jwt": {
    "Key": "change-this-secret-key-min-32-chars",
    "Issuer": "CommunityEventsApi",
    "Audience": "CommunityEventsClient"
  }
}
```

##  Troubleshooting

### "Couldn't find a project to run"
Always run from root with full path:
```powershell
dotnet run --project src\CommunityEventsApi\CommunityEventsApi.csproj
```

### Port already in use
Check console output for actual ports or update `Properties/launchSettings.json`

### Database connection failed
- Start SQL Server LocalDB: `sqllocaldb start mssqllocaldb`
- Verify connection string in `appsettings.json`

##  Full Documentation

For detailed documentation, see:
- **Swagger UI**: https://localhost:PORT/swagger
- **QUICK_START.md**: Complete setup guide
- **Architecture**: Clean architecture with DAL/BAL separation

##  Contributing

1. Fork the repo
2. Create feature branch
3. Commit changes
4. Push and create PR

##  License

MIT License

---

**Author**: [jayantajm1](https://github.com/jayantajm1)

**Quick Links**: [Swagger](https://localhost:50126/swagger) | [Issues](https://github.com/jayantajm1/community-event-ind-api/issues)
