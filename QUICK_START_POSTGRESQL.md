# PostgreSQL Setup - Quick Commands

## ✅ Configuration Updated

Your project is now configured for PostgreSQL:
- **Database**: `community_events_platform`
- **Username**: `postgres`
- **Password**: `postgres`
- **Provider**: Npgsql.EntityFrameworkCore.PostgreSQL 8.0.0

---

## 🚀 Quick Start Commands

### 1. Stop Running Application (if any)

The application might be running from a previous terminal. Stop it first.

### 2. Create Initial Migration

```powershell
dotnet ef migrations add InitialCreate --project src\CommunityEventsApi --output-dir Data/Migrations
```

### 3. Apply Migration to Database

```powershell
dotnet ef database update --project src\CommunityEventsApi
```

### 4. Run the Application

```powershell
dotnet run --project src\CommunityEventsApi\CommunityEventsApi.csproj
```

---

## 📊 Scaffold Commands (Alternative Options)

### Option A: Reverse Engineer Existing Database

If you already have tables in PostgreSQL:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Database=community_events_platform;Username=postgres;Password=postgres" Npgsql.EntityFrameworkCore.PostgreSQL --project src\CommunityEventsApi --context-dir Data --output-dir Models --context ApplicationDbContext --force
```

### Option B: Scaffold Using Connection String Name

```powershell
dotnet ef dbcontext scaffold Name=DefaultConnection Npgsql.EntityFrameworkCore.PostgreSQL --project src\CommunityEventsApi --context-dir Data --output-dir Models --context ApplicationDbContext --force
```

---

## 🔍 Verify Setup

### Check EF Core Tools Version
```powershell
dotnet ef --version
```

### List Migrations
```powershell
dotnet ef migrations list --project src\CommunityEventsApi
```

### Check Database Connection
```powershell
psql -h localhost -U postgres -d community_events_platform -c "\dt"
```

---

## 📝 What Was Changed

1. **appsettings.json** - Updated connection string to PostgreSQL format
2. **CommunityEventsApi.csproj** - Replaced SQL Server packages with:
   - `Npgsql.EntityFrameworkCore.PostgreSQL` (v8.0.0)
   - `Microsoft.EntityFrameworkCore.Design` (v8.0.0)
3. **Program.cs** - Changed from `UseSqlServer()` to `UseNpgsql()`

---

## 🎯 Recommended Next Steps

1. Create migration: `dotnet ef migrations add InitialCreate --project src\CommunityEventsApi`
2. Update database: `dotnet ef database update --project src\CommunityEventsApi`
3. Run application: `dotnet run --project src\CommunityEventsApi\CommunityEventsApi.csproj`
4. Access Swagger: `https://localhost:50126/swagger`

---

## 🔧 Troubleshooting

### "No database provider configured"
- Make sure you ran: `dotnet restore`

### "Cannot connect to database"
- Verify PostgreSQL is running: `Get-Service -Name postgresql*`
- Test connection: `psql -U postgres -c "SELECT version();"`

### "Build failed"
- Stop any running instances of the application
- Clean build: `dotnet clean; dotnet build`

---

**Ready to create your database!** 🎉

Run: `dotnet ef migrations add InitialCreate --project src\CommunityEventsApi`
