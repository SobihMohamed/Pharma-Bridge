using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.OrdersOperationsConfig
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            //تم تعبئة الكرش بنجاح 
            builder.ToTable("Complaints");

            builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).IsRequired().HasMaxLength(500);
            builder.Property(c => c.AdminNotes).HasMaxLength(500);

            builder.Property(c => c.Status)
                   .HasConversion(
                       v => v.ToString(),
                       v => (ComplaintStatus)Enum.Parse(typeof(ComplaintStatus), v)
                   ).HasMaxLength(50);

            //  Relationships

            // Complaint -> Order
            builder.HasOne(c => c.Order)
                   .WithMany(o => o.Complaints)
                   .HasForeignKey(c => c.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Complaint -> SubmittedBy (ApplicationUser)
            builder.HasOne(c => c.SubmittedBy)
                   .WithMany(u => u.Complaints)
                   .HasForeignKey(c => c.SubmittedById)
                   .OnDelete(DeleteBehavior.Restrict);

            // Complaint -> ResolvedBy (ApplicationUser)
            builder.HasOne(c => c.ResolvedBy)
                   .WithMany(u => u.ResolvedComplaints)
                   .HasForeignKey(c => c.ResolvedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
