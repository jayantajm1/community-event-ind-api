using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Community> Communities { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventRegistration> EventRegistrations { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<EventCategory> EventCategories { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Membership> Memberships { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<EventRegistration>()
            .HasIndex(er => new { er.EventId, er.UserId })
            .IsUnique();

        modelBuilder.Entity<Membership>()
            .HasIndex(m => new { m.CommunityId, m.UserId })
            .IsUnique();

        // Configure relationships
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Creator)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.Community)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
