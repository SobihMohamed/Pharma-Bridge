using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Infrastructure.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.ToTable("Bids");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Subtotal)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.DiscountAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.DeliveryFee)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.PlatformFee)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.TotalPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasMaxLength(50) // In Review
                   .IsRequired() // In Review
                   .HasConversion(
                       v => v.ToString(),
                       v => (BidStatus)Enum.Parse(typeof(BidStatus), v)
                   );

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);

            builder.Property(x => x.SubmittedAt)
                   .IsRequired();

            builder.Property(x => x.RespondedAt);

            builder.Property(x => x.DeliveryTimeInMinutes)
                   .IsRequired();

            // Relations 

            // 12 - Bid (Many) To (1) Pharmacy 

            builder.HasOne(x => x.Pharmacy)
                   .WithMany(p => p.Bids)
                   .HasForeignKey(x => x.PharmacyId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 20 - PrescriptionRequest (1) To (Many) Bid (Has)

            builder.HasOne(x => x.PrescriptionRequest)
                   .WithMany(pr => pr.Bids)
                   .HasForeignKey(x => x.PrescriptionRequestId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
}