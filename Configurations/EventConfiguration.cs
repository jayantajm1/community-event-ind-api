//using CommunityEventsApi.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace CommunityEventsApi.Configurations
//{

//    public class EventConfiguration : IEntityTypeConfiguration<Event>
//    {
//        public void Configure(EntityTypeBuilder<Event> builder)
//        {
//            // 🔑 Primary Key
//            builder.HasKey(e => e.Id);

//            // 📝 Basic Properties
//            builder.Property(e => e.Title)
//                .IsRequired()
//                .HasMaxLength(200);

//            builder.Property(e => e.Description)
//                .IsRequired()
//                .HasMaxLength(2000);

//            builder.Property(e => e.EventType)
//                .IsRequired()
//                .HasMaxLength(50);

//            builder.Property(e => e.RegistrationMode)
//                .IsRequired()
//                .HasMaxLength(50);

//            builder.Property(e => e.Visibility)
//                .IsRequired()
//                .HasMaxLength(50);

//            builder.Property(e => e.Status)
//                .IsRequired()
//                .HasMaxLength(50)
//                .HasDefaultValue("upcoming"); // ✅ lowercase (recommended)

//            // ⏰ Date fields
//            builder.Property(e => e.StartDatetime)
//                .IsRequired();

//            builder.Property(e => e.EndDatetime)
//                .IsRequired();

//            // 🌍 Location text fields
//            builder.Property(e => e.LocationName)
//                .HasMaxLength(200);

//            builder.Property(e => e.Address)
//                .HasMaxLength(500);

//            // 🔗 Event → Community
//            builder.HasOne(e => e.Community)
//                .WithMany(c => c.Events)
//                .HasForeignKey(e => e.CommunityId)
//                .OnDelete(DeleteBehavior.Cascade);

//            // 🔗 Event → Organizer (User)
//            builder.HasOne(e => e.Organizer)
//                .WithMany(u => u.CreatedEvents)
//                .HasForeignKey(e => e.OrganizerId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // 📌 Indexes (performance)
//            builder.HasIndex(e => e.CommunityId);
//            builder.HasIndex(e => e.OrganizerId);
//            builder.HasIndex(e => e.StartDatetime);
//        }
//    }
//}
