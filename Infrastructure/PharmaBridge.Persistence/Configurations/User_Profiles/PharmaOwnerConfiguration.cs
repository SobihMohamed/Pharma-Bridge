using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.User_Profiles
{
    public class PharmaOwnerConfiguration : IEntityTypeConfiguration<PharmaOwner>
    {
        public void Configure(EntityTypeBuilder<PharmaOwner> builder)
        {
            builder.ToTable("PharmaOwners");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NationalId)
                .HasMaxLength(20);

            builder.Property(x => x.NationalIdFront)
                .HasMaxLength(500);

            builder.Property(x => x.NationalIdBack)
                .HasMaxLength(500);

            builder.Property(x => x.SyndicateCardImage)
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired()
                .HasConversion<string>();
            
            //The relation of between PharmaOwner and Pharmacy will be added in the class of Config Pharmacy

            builder.HasOne(x => x.ApplicationUser)
                   .WithOne(u => u.PharmaOwnerProfile)
                   .HasForeignKey<PharmaOwner>(x => x.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
        }
    }
