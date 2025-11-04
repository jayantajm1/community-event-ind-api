<<<<<<< HEAD
# Community Events API

A comprehensive ASP.NET Core Web API for managing community events, user interactions, and event registrations.

## 🏗️ Architecture

This project follows a clean architecture pattern with clear separation of concerns:

- **Controllers**: API endpoints layer
- **BAL (Business Access Layer)**: Business logic and services
- **DAL (Data Access Layer)**: Data access with Repository pattern
- **DTOs**: Data Transfer Objects for API requests/responses
- **Models**: Entity models (Database tables)
- **Middleware**: Cross-cutting concerns (logging, exception handling, authentication)
- **Helpers & Utils**: Common utilities and helper functions

## 🚀 Features

- **Authentication & Authorization**: JWT-based authentication
- **User Management**: User registration, profile management
- **Community Management**: Create and manage communities
- **Event Management**: CRUD operations for events
- **Event Registration**: Users can register/unregister for events
- **Comments**: Comment system for events
- **Geolocation**: Find nearby events based on location
- **File Upload**: Support for image uploads (events, profiles)

## 🛠️ Tech Stack

- **.NET 8.0**
- **Entity Framework Core** (SQL Server)
- **AutoMapper** (Object-to-object mapping)
- **JWT Authentication**
- **Swagger/OpenAPI** (API documentation)
- **xUnit** (Unit testing)
- **Moq & FluentAssertions** (Testing libraries)

## 📁 Project Structure

```
CommunityEventsApi/
├── src/
│   ├── CommunityEventsApi/              # Main API project
│   │   ├── Controllers/                 # API endpoints
│   │   ├── Middleware/                  # Custom middleware
│   │   ├── Helpers/                     # Helper classes
│   │   ├── Utils/                       # Utility functions
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Models/                      # Entity models
│   │   ├── Data/                        # DbContext
│   │   ├── DAL/                         # Data Access Layer
│   │   ├── BAL/                         # Business Access Layer
│   │   ├── Mappings/                    # AutoMapper profiles
│   │   ├── Observability/               # Logging & monitoring
│   │   └── Configurations/              # EF configurations
│   │
│   └── CommunityEventsApi.Tests/        # Unit & Integration Tests
│       ├── Services/
│       ├── Controllers/
│       ├── Repositories/
│       └── Mocks/
│
├── CommunityEventsApi.sln               # Solution file
└── docker-compose.yml                   # Docker configuration
```

## 🔧 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd community-event-ind-api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update connection string**
   
   Edit `src/CommunityEventsApi/appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your-Connection-String-Here"
   }
   ```

4. **Run database migrations**
   ```bash
   cd src/CommunityEventsApi
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/CommunityEventsApi
   ```

   Or use watch mode for development:
   ```bash
   dotnet watch --project src/CommunityEventsApi
   ```

6. **Access Swagger UI**
   
   Navigate to: `https://localhost:5001/swagger`

## 🐳 Docker Deployment

```bash
docker-compose up -d
```

This will start:
- Community Events API on port 5000
- SQL Server on port 1433

## 🧪 Running Tests

```bash
dotnet test
```

## 📝 API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/signup` - User registration
- `POST /api/auth/refresh-token` - Refresh access token

### Users
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user profile
- `DELETE /api/users/{id}` - Delete user

### Communities
- `GET /api/communities` - Get all communities
- `GET /api/communities/{id}` - Get community by ID
- `POST /api/communities` - Create community
- `PUT /api/communities/{id}` - Update community
- `DELETE /api/communities/{id}` - Delete community

### Events
- `GET /api/events` - Get all events (with filters)
- `GET /api/events/{id}` - Get event by ID
- `POST /api/events` - Create event
- `PUT /api/events/{id}` - Update event
- `DELETE /api/events/{id}` - Delete event
- `POST /api/events/{id}/register` - Register for event
- `GET /api/events/nearby` - Get nearby events

### Comments
- `GET /api/comments/event/{eventId}` - Get comments for event
- `POST /api/comments` - Create comment
- `PUT /api/comments/{id}` - Update comment
- `DELETE /api/comments/{id}` - Delete comment

## 🔐 Authentication

The API uses JWT Bearer tokens for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-token-here>
```

## 📊 Database Schema

Key entities:
- **Users**: User accounts and profiles
- **Communities**: Community organizations
- **Events**: Event information with location
- **EventRegistrations**: User event registrations
- **Comments**: Event comments
- **EventCategories**: Event categorization
- **Locations**: Geolocation data
- **Memberships**: Community memberships

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👥 Authors

Your Name - Community Events Team

## 🙏 Acknowledgments

- ASP.NET Core documentation
- Entity Framework Core documentation
- Community contributors
=======
A community-driven events platform where community leaders/hosts can create events (with photos, description, registration), members join events with their community ID, comment and ask questions, search events by location/type, and where admins moderate and close events after completion.
>>>>>>> fe92b33d3d893be9fe98d19bca98778e77593af3
