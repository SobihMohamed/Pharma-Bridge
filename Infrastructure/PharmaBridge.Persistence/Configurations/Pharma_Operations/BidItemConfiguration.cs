using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Persistence.Configurations.Pharma_Operations
{
    public class BidItemConfiguration : IEntityTypeConfiguration<BidItem>
    {
        public void Configure(EntityTypeBuilder<BidItem> builder)
        {
            builder.ToTable("BidItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ItemName)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.UnitPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.IsAlternative)
                   .IsRequired();

            builder.Property(x => x.AlternativeNote)
                   .HasMaxLength(500);

            builder.Property(x => x.LineTotal)
                   .HasPrecision(18, 2)
                   .IsRequired();

            // Relations 

            // 11 - Bid (1) To (Many) BidItems
            // child table is useless without the parent table

            builder.HasOne(x => x.Bid)
                   .WithMany(b => b.BidItems)
                   .HasForeignKey(x => x.BidId)
                   .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}