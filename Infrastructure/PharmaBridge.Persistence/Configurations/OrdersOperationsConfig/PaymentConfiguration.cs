using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Persistence.Configurations.OrdersOperationsConfig
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            //تم تعبئة الكرش بنجاح 
            builder.ToTable("Payments");

            builder.Property(p => p.Amount).HasPrecision(18, 2);
            builder.Property(p => p.PaymentIntentId).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PaymentMethod).IsRequired().HasMaxLength(100);
            builder.Property(p => p.GatewayName).HasMaxLength(100);
            builder.Property(p => p.GatewayResponse).HasMaxLength(500);

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            // Relationships

            // Payment -> Order
            builder.HasOne(p => p.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
