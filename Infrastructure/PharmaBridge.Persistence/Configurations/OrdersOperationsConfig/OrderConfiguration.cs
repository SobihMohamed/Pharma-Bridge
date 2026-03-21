using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.OrdersOperationsConfig
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //تم تعبئة الكرش بنجاح 
            builder.ToTable("Orders");

            builder.Property(o => o.Amount).HasPrecision(18, 2);
            builder.Property(o => o.PaymentMethod).IsRequired().HasMaxLength(100);
            builder.Property(o => o.CancelReason).HasMaxLength(500);

            builder.Property(o => o.OrderStatus)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // Relationships 

            // Order -> PatientAddress
            builder.HasOne(o => o.PatientAddress)
                   .WithMany()
                   .HasForeignKey(o => o.PatientAddressId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order -> Bid (1 to 1)
            builder.HasOne(o => o.Bid)
                   .WithOne(b => b.Order)
                   .HasForeignKey<Order>(o => o.BidId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order -> PatientProfile
            builder.HasOne(o => o.PatientProfile)
                   .WithMany(p => p.Orders)
                   .HasForeignKey(o => o.PatientProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order -> Pharmacy
            builder.HasOne(o => o.Pharmacy)
                   .WithMany(p => p.Orders)
                   .HasForeignKey(o => o.PharmacyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Order -> PrescriptionRequest (1 to 1)
            builder.HasOne(o => o.PrescriptionRequest)
                   .WithOne(pr => pr.Order)
                   .HasForeignKey<Order>(o => o.PrescriptionRequestId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
