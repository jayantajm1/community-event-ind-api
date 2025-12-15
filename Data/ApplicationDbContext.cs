using System;
using System.Collections.Generic;
using CommunityEventsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventsApi.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Community> Communities { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<ViewUpcomingEvent> ViewUpcomingEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("postgis")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs");

            entity.HasIndex(e => e.ActionType, "idx_audit_action");

            entity.HasIndex(e => e.ActorUserId, "idx_audit_actor");

            entity.HasIndex(e => new { e.TargetType, e.TargetId }, "idx_audit_target");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(50)
                .HasColumnName("action_type");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Metadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.TargetId).HasColumnName("target_id");
            entity.Property(e => e.TargetType)
                .HasMaxLength(50)
                .HasColumnName("target_type");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ActorUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("audit_logs_actor_user_id_fkey");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comments_pkey");

            entity.ToTable("comments");

            entity.HasIndex(e => e.Content, "idx_comments_content_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.EventId, "idx_comments_event");

            entity.HasIndex(e => e.ParentCommentId, "idx_comments_parent");

            entity.HasIndex(e => e.UserId, "idx_comments_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Attachments).HasColumnName("attachments");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsHidden)
                .HasDefaultValue(false)
                .HasColumnName("is_hidden");
            entity.Property(e => e.ParentCommentId).HasColumnName("parent_comment_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.Comments)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("comments_event_id_fkey");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("comments_parent_comment_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("comments_user_id_fkey");
        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("communities_pkey");

            entity.ToTable("communities");

            entity.HasIndex(e => e.Slug, "communities_slug_key").IsUnique();

            entity.HasIndex(e => e.Name, "idx_communities_name_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.Visibility, "idx_communities_visibility");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .HasColumnName("region");
            entity.Property(e => e.Slug)
                .HasMaxLength(150)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Visibility)
                .HasMaxLength(20)
                .HasDefaultValueSql("'public'::character varying")
                .HasColumnName("visibility");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Communities)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("communities_created_by_fkey");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("events_pkey");

            entity.ToTable("events");

            entity.HasIndex(e => e.Slug, "events_slug_key").IsUnique();

            entity.HasIndex(e => e.CommunityId, "idx_events_community");

            entity.HasIndex(e => new { e.StartDatetime, e.EndDatetime }, "idx_events_datetime");

            entity.HasIndex(e => e.OrganizerId, "idx_events_organizer");

            entity.HasIndex(e => e.Status, "idx_events_status");

            entity.HasIndex(e => e.Tags, "idx_events_tags_gin").HasMethod("gin");

            entity.HasIndex(e => e.Title, "idx_events_title_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.EventType, "idx_events_type");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDatetime).HasColumnName("end_datetime");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .HasColumnName("event_type");
            entity.Property(e => e.Images)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("images");
            entity.Property(e => e.LocationName)
                .HasMaxLength(255)
                .HasColumnName("location_name");
            entity.Property(e => e.MaxParticipants)
                .HasDefaultValue(0)
                .HasColumnName("max_participants");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.RegistrationMode)
                .HasMaxLength(20)
                .HasDefaultValueSql("'auto'::character varying")
                .HasColumnName("registration_mode");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.StartDatetime).HasColumnName("start_datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Tags)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("tags");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Visibility)
                .HasMaxLength(20)
                .HasDefaultValueSql("'public'::character varying")
                .HasColumnName("visibility");

            entity.HasOne(d => d.Community).WithMany(p => p.Events)
                .HasForeignKey(d => d.CommunityId)
                .HasConstraintName("events_community_id_fkey");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .HasConstraintName("events_organizer_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.IsRead, "idx_notifications_isread");

            entity.HasIndex(e => e.Type, "idx_notifications_type");

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Payload)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("payload");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("registrations_pkey");

            entity.ToTable("registrations");

            entity.HasIndex(e => e.EventId, "idx_registrations_event");

            entity.HasIndex(e => e.Status, "idx_registrations_status");

            entity.HasIndex(e => e.UserId, "idx_registrations_user");

            entity.HasIndex(e => new { e.EventId, e.UserId }, "uniq_registration_event_user").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CheckedInAt).HasColumnName("checked_in_at");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.RegisteredAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("registered_at");
            entity.Property(e => e.RegistrationData)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("registration_data");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'registered'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Community).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.CommunityId)
                .HasConstraintName("registrations_community_id_fkey");

            entity.HasOne(d => d.Event).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("registrations_event_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("registrations_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.CommunityId, "idx_users_community");

            entity.HasIndex(e => e.Role, "idx_users_role");

            entity.HasIndex(e => e.Username, "idx_users_username_trgm")
                .HasMethod("gin")
                .HasOperators(new[] { "gin_trgm_ops" });

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CommunityId).HasColumnName("community_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.LastSeen).HasColumnName("last_seen");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'member'::character varying")
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValueSql("'active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Community).WithMany(p => p.Users)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_users_community");
        });

        modelBuilder.Entity<ViewUpcomingEvent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("view_upcoming_events");

            entity.Property(e => e.CommunityName)
                .HasMaxLength(150)
                .HasColumnName("community_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Organizer)
                .HasMaxLength(100)
                .HasColumnName("organizer");
            entity.Property(e => e.StartDatetime).HasColumnName("start_datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
