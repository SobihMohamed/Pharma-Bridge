using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmaRequestsConfig
{
    public class PrescriptionRequestHistoriesConfiguration : IEntityTypeConfiguration<PrescriptionRequestHistory>
    {
        public void Configure(EntityTypeBuilder<PrescriptionRequestHistory> builder)
        {
            // Table & Key

            builder.ToTable("PrescriptionRequestHistories");

            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.Notes)
                   .HasMaxLength(500)
                   .IsRequired();


            builder.Property(x => x.NewStatus)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.OldStatus)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.ChangedAt)
                   .IsRequired();

            // Relationships

            // PrescriptionRequestHistory (Many) → (1) PrescriptionRequest

            builder.HasOne(x => x.PrescriptionRequest)
                   .WithMany(p => p.PrescriptionRequestHistorys)
                   .HasForeignKey(x => x.PrescriptionRequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            // PrescriptionRequestHistory (Many) → (1) ApplicationUser (ChangedBy)

            builder.HasOne(x => x.ChangedBy)
                   .WithMany()
                   .HasForeignKey(x => x.ChangedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
