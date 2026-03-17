using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;


namespace PharmaBridge.Persistence.Configurations
{
    public class PharmaPerformSnapshotConfiguration : IEntityTypeConfiguration<PharmaPerformSnapshot>
    {
        public void Configure(EntityTypeBuilder<PharmaPerformSnapshot> builder)
        {
            builder.ToTable("PharmaPerformSnapshots");

            builder.HasKey(x=>x.Id);

            builder.Property(x=>x.PeriodStart)
                .IsRequired();

            builder.Property(x => x.PeriodEnd)
                .IsRequired();

            builder.Property(x => x.PeriodType)
                   .HasMaxLength(50)
                   .IsRequired()
                   .HasConversion(
                       v => v.ToString(),
                       v => (SnapshotPeriodType)Enum.Parse(typeof(SnapshotPeriodType), v)
                   );


            builder.Property(x => x.TotalBids)
                .IsRequired();

            builder.Property(x => x.WonOrders)
                .IsRequired();

            builder.Property(x => x.CompletedOrders)
                   .IsRequired();

            builder.Property(x => x.CancelledOrders)
                   .IsRequired();

            builder.Property(x => x.CompletionRate)
                   .HasPrecision(5, 2)
                   .IsRequired();

            builder.Property(x => x.TotalRevenue)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.TotalPlatformFee)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.ComputedAt)
                   .IsRequired();

            // Relations

            // PharmaPerformSnapshot (Many)  Pharmacy (1) 

            builder.HasOne(x => x.Pharmacy)
                   .WithMany(p => p.PharmaPerformSnapshots)
                   .HasForeignKey(x => x.PharmacyId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
