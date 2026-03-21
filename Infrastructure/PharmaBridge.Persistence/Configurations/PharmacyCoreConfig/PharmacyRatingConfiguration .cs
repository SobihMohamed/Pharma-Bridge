using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmacyCoreConfig
{
    public class PharmacyRatingConfiguration : IEntityTypeConfiguration<PharmacyRating>
    {
        public void Configure(EntityTypeBuilder<PharmacyRating> builder)
        {
            builder.ToTable("PharmacyRatings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RatingValue)
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasMaxLength(500);



            builder.HasOne(x => x.Pharmacy)
                   .WithMany(p => p.PharmacyRatings)
                   .HasForeignKey(x => x.PharmacyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PatientProfile)
                   .WithMany(p => p.PharmacyRatings)
                   .HasForeignKey(x => x.PatientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
