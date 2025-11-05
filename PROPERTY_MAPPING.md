# Property Name Mapping - Scaffolded Models vs Original

## User Model Changes
| Original Property | Scaffolded Property | Type |
|-------------------|---------------------|------|
| FirstName + LastName | FullName | string |
| PhoneNumber | Phone | string? |
| ProfileImageUrl | AvatarUrl | string? |
| ProfilePictureUrl | AvatarUrl | string? |

## Event Model Changes
| Original Property | Scaffolded Property | Type |
|-------------------|---------------------|------|
| Creator | Organizer | User navigation |
| CreatedBy | OrganizerId | Guid |
| Location | LocationName + Address | string properties (no Location entity) |
| Category | N/A | Removed |
| CategoryId | N/A | Removed |

## Community Model Changes
| Original Property | Scaffolded Property | Type |
|-------------------|---------------------|------|
| ImageUrl | N/A | Removed (not in DB) |
| ProfilePictureUrl | N/A | Removed (not in DB) |

## ApplicationDbContext Changes
| Original DbSet | Scaffolded DbSet | Notes |
|----------------|------------------|-------|
| Memberships | N/A | Removed - doesn't exist in DB |
| EventRegistrations | Registrations | Renamed |
| EventCategories | N/A | Removed - doesn't exist in DB |
| Locations | N/A | Removed - location is inline in Event |

## New Models from Scaffold
- **AuditLog** - Audit trail tracking
- **Notification** - User notifications
- **Registration** - Event registrations (replaces EventRegistration)
- **Comment** - Has ParentComment support for threading
- **ViewUpcomingEvent** - Database view for upcoming events

## API Impact

The API endpoints will continue to work, but the DTOs need to be aware that:
1. User FullName is a single field (not FirstName + LastName)
2. Phone instead of PhoneNumber
3. AvatarUrl instead of ProfileImageUrl/ProfilePictureUrl
4. Event has Organizer/OrganizerId instead of Creator/CreatedBy
5. Event location is stored as LocationName + Address (no separate Location entity)
6. No EventCategories in the database
7. Registrations instead of EventRegistrations

## Quick Fix: Run Without Fixing Code

The current code has mismatches. You have two options:

### Option 1: Use the scaffolded models as-is (RECOMMENDED - Database is already set up)
- The database schema is correct
- Update services/configurations to match scaffolded properties
- This is what we should do

### Option 2: Recreate migrations from original models
- Drop the database
- Delete scaffolded models
- Restore original model files
- Create fresh migrations
- Lose the PostgreSQL database structure

**Recommendation: Use Option 1** - The database is set up correctly, we just need to align the code.
