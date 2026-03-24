using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmaRequestsConfig
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Table & Key

            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.NotifyType)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.Title)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(x => x.ReferenceType)
                   .HasMaxLength(100);

            builder.Property(x => x.IsRead)
                   .IsRequired();

            // Relationships

            // Notification (Many) → (1) ApplicationUser

            builder.HasOne(x => x.ApplicationUser)
                   .WithMany(u => u.Notifications)
                   .HasForeignKey(x => x.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
