using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmaRequestsConfig
{
    public class PrescriptionRequestConfiguration : IEntityTypeConfiguration<PrescriptionRequestEntity>
    {
        public void Configure(EntityTypeBuilder<PrescriptionRequestEntity> builder)
        {
            // Table & Key

            builder.ToTable("PrescriptionRequests", table =>
            {
                table.HasCheckConstraint(
                    "CK_PrescriptionRequest_Content",
                    "[ImageUrl] IS NOT NULL OR [MedicineName] IS NOT NULL");
            });

            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(x => x.PatientNotes)
                   .HasMaxLength(500);

            builder.Property(x => x.MedicineName)
                   .HasMaxLength(100);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.ExpiresAt)
                   .IsRequired();

            // Relationships

            // PrescriptionRequest (Many) → (1) PatientProfile
            builder.HasOne(x => x.PatientProfile)
                   .WithMany(p => p.PrescriptionRequests)
                   .HasForeignKey(x => x.PatientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            // PrescriptionRequest (Many) → (1) DeliveryAddress
            builder.HasOne(x => x.DeliveryAddress)
                   .WithMany()
                   .HasForeignKey(x => x.DeliveryAddressId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
