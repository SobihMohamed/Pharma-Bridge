using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.User_Profiles
{
    public class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            builder.ToTable("PatientProfiles");

            builder.HasKey(x => x.Id);

            //The Relations of Orders, PatientAddresses, PrescriptionRequests and PharmacyRatings will be added in their config class

            builder.HasOne(x => x.ApplicationUser)
                   .WithOne(u => u.PatientProfile)
                   .HasForeignKey<PatientProfile>(x => x.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
