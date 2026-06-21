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
                   .HasMaxLength(100);

            builder.Property(x => x.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.LicenseImageUrl)
                   .HasMaxLength(500);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.Latitude)
                   .IsRequired()
                   .HasPrecision(18, 8);

            builder.Property(x => x.Longitude)
                   .IsRequired()
                   .HasPrecision(18, 8);

            builder.Property(x => x.OpenTime);

            builder.Property(x => x.CloseTime);

            builder.Property(x => x.Is24Hours)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(x => x.TextAddress)
                   .HasMaxLength(500);

            builder.Property(x => x.ContactPhone)
                   .HasMaxLength(11);

            builder.Property(x => x.AverageRating)
                   .IsRequired()
                   .HasPrecision(3, 2);

            builder.Property(x => x.CompleteOrderCount)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(x => x.RejectedReasons)
                   .HasMaxLength(500);

            builder.HasOne(x => x.PharmaOwner)
                   .WithOne(o => o.Pharmacy)
                   .HasForeignKey<Pharmacy>(x => x.PharmaOwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
