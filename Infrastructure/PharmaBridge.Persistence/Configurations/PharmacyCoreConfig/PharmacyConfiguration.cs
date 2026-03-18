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

            builder.Property(x => x.ContactPhone)
                   .HasMaxLength(11);

            builder.Property(x => x.AverageRating)
                   .HasPrecision(18, 2);

            builder.Property(x => x.CompleteOrderCount)
                   .HasDefaultValue(0);

            builder.Property(x => x.RejectedReasons)
                   .HasMaxLength(500);

            builder.HasOne(x => x.PharmaOwner)
                   .WithMany(o => o.Pharmacies)
                   .HasForeignKey(x => x.PharmaOwnerId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
