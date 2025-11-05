# PostgreSQL Database Scaffold Guide

## 📋 Prerequisites

Your database configuration:
- **Database Name**: `community_events_platform`
- **Username**: `postgres`
- **Password**: `postgres`
- **Host**: `localhost` (default)
- **Port**: `5432` (default)

## 🔧 Setup Steps

### Step 1: Install EF Core Tools (if not already installed)

```powershell
dotnet tool install --global dotnet-ef
```

Or update existing:
```powershell
dotnet tool update --global dotnet-ef
```

### Step 2: Restore NuGet Packages

```powershell
dotnet restore src\CommunityEventsApi\CommunityEventsApi.csproj
```

## 🚀 Scaffold Commands

### Option 1: Scaffold Existing Database to DbContext

If you already have tables in your PostgreSQL database and want to reverse-engineer them:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Database=community_events_platform;Username=postgres;Password=postgres" Npgsql.EntityFrameworkCore.PostgreSQL --project src\CommunityEventsApi --context-dir Data --output-dir Models --context ApplicationDbContext --force
```

**Parameters Explained:**
- `--project src\CommunityEventsApi` - Target project
- `--context-dir Data` - Directory for DbContext file
- `--output-dir Models` - Directory for entity models
- `--context ApplicationDbContext` - Name of the DbContext class
- `--force` - Overwrite existing files

### Option 2: Create Migrations from Existing Models (Recommended)

Since we already have models defined, create migrations to generate the database schema:

```powershell
# Navigate to project directory
cd src\CommunityEventsApi

# Create initial migration
dotnet ef migrations add InitialCreate --output-dir Data/Migrations

# Apply migrations to database
dotnet ef database update
```

### Option 3: Scaffold Specific Tables Only

If you want to scaffold only specific tables:

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Database=community_events_platform;Username=postgres;Password=postgres" Npgsql.EntityFrameworkCore.PostgreSQL --project src\CommunityEventsApi --context-dir Data --output-dir Models --context ApplicationDbContext --table users --table events --table communities --force
```

## 📝 Alternative: Using appsettings.json Connection String

Instead of typing the connection string in command, you can reference it:

```powershell
dotnet ef dbcontext scaffold Name=DefaultConnection Npgsql.EntityFrameworkCore.PostgreSQL --project src\CommunityEventsApi --context-dir Data --output-dir Models --context ApplicationDbContext --force
```

## ✅ Recommended Workflow

Since you already have entity models defined, I recommend **Option 2**:

### Step-by-Step:

1. **Ensure PostgreSQL is running**:
   ```powershell
   # Check if PostgreSQL service is running
   Get-Service -Name postgresql*
   ```

2. **Create the database** (if it doesn't exist):
   ```sql
   -- Connect to PostgreSQL as postgres user
   CREATE DATABASE community_events_platform;
   ```

3. **Build the project**:
   ```powershell
   dotnet build src\CommunityEventsApi\CommunityEventsApi.csproj
   ```

4. **Create migrations**:
   ```powershell
   dotnet ef migrations add InitialCreate --project src\CommunityEventsApi --output-dir Data/Migrations
   ```

5. **Update database**:
   ```powershell
   dotnet ef database update --project src\CommunityEventsApi
   ```

6. **Verify tables created**:
   ```sql
   -- Connect to PostgreSQL
   \c community_events_platform
   \dt
   ```

## 🔍 Verify DbContext

After scaffolding or migration, check that `ApplicationDbContext` is properly configured:

```csharp
// Should be in: src/CommunityEventsApi/Data/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<EventCategory> EventCategories { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Membership> Memberships { get; set; }
}
```

## 🛠️ Troubleshooting

### Connection Failed

If you get connection errors:

1. **Check PostgreSQL is running**:
   ```powershell
   psql -U postgres -c "SELECT version();"
   ```

2. **Test connection string**:
   ```powershell
   psql -h localhost -U postgres -d community_events_platform
   ```

3. **Verify pg_hba.conf** allows local connections

### Migration Errors

If migrations fail:

```powershell
# Drop and recreate database
dotnet ef database drop --project src\CommunityEventsApi --force
dotnet ef database update --project src\CommunityEventsApi
```

### Provider Not Found

If you get "No database provider configured":

```powershell
# Verify packages are installed
dotnet list src\CommunityEventsApi\CommunityEventsApi.csproj package
```

Should show:
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`

## 📊 PostgreSQL-Specific Notes

### Data Type Mappings

PostgreSQL uses different conventions:
- Table names: lowercase with underscores (e.g., `event_categories`)
- Column names: lowercase with underscores (e.g., `created_at`)

### To Use Snake Case (Optional)

Install:
```powershell
dotnet add src\CommunityEventsApi package EFCore.NamingConventions
```

Update Program.cs:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());
```

## 🎯 Quick Reference

**Connection String Format:**
```
Host=localhost;Database=community_events_platform;Username=postgres;Password=postgres;Port=5432
```

**Common Commands:**
```powershell
# Check EF Tools version
dotnet ef --version

# List migrations
dotnet ef migrations list --project src\CommunityEventsApi

# Remove last migration
dotnet ef migrations remove --project src\CommunityEventsApi

# Generate SQL script
dotnet ef migrations script --project src\CommunityEventsApi --output migration.sql
```

---

**Ready to go!** Run the migration commands and your PostgreSQL database will be set up with all entities. 🚀
