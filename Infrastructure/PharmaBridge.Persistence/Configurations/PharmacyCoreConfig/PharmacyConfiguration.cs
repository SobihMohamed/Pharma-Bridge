using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmacyCoreConfig
{
    public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
    {
        public void Configure(EntityTypeBuilder<Pharmacy> builder)
        {

            builder.ToTable("Pharmacies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PharmacyName)
                   .IsRequired()
                   .HasMaxLength(70);

            builder.Property(x => x.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.LicenseImageUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.Latitude)
                   .HasPrecision(18, 2);

            builder.Property(x => x.Longitude)
                   .HasPrecision(18, 2);

            builder.Property(x => x.OpenTime);

            builder.Property(x => x.CloseTime);

            builder.Property(x => x.Is24Hours)
                   .HasDefaultValue(false);

            builder.Property(x => x.TextAddress)
                   .HasMaxLength(250);

            builder.Property(p => p.ContactPhone)
                   .HasMaxLength(11);

            builder.Property(p => p.AverageRating)
                   .HasPrecision(18, 2);

            builder.Property(p => p.CompleteOrderCount)
                   .HasDefaultValue(0);

            builder.Property(p => p.RejectedReasons)
                   .HasMaxLength(500);

            builder.HasOne(p => p.PharmaOwner)
                   .WithMany(o => o.Pharmacies)
                   .HasForeignKey(p => p.PharmaOwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
