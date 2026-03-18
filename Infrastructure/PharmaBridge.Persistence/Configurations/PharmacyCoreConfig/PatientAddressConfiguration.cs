using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.PharmacyCoreConfig
{
    public class PatientAddressConfiguration : IEntityTypeConfiguration<PatientAddress>
    {
        public void Configure(EntityTypeBuilder<PatientAddress> builder)
        {
            builder.ToTable("PatientAddresses");


            builder.HasKey(x => x.Id);

            builder.Property(x => x.AddressLine)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Latitude)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.Longitude)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.IsDefault)
                   .IsRequired()
                   .HasDefaultValue(false);


            builder.HasOne(x => x.PatientProfile)
                   .WithMany(p => p.PatientAddresses)
                   .HasForeignKey(x => x.PatientProfileId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
