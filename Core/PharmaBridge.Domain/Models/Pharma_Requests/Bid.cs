using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class Bid : BaseEntity<int>
    {
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; } = 0.00m;
        public decimal DeliveryFee { get; set; } = 0.00m; //  In Review
        public decimal PlatformFee { get; set; } = 0.00m; // TODO: IN FUTURE 
        public decimal TotalPrice { get; set; }
        public BidStatus Status { get; set; } = BidStatus.Pending;
        public string? Notes { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
        public int DeliveryTimeInMinutes { get; set; }

        // 11 - Bid (1) To (Many) BidItems (Contains)
        public virtual ICollection<BidItem> BidItems { get; set; } = new HashSet<BidItem>();

        // 12 - Bid (Many) To (1) Pharmacy (Create Bid)
        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        // 14 - Order (1) To (1) Bid (Converted To)
        // nullable because the bid can be created and not converted to order yet
        public virtual Order? Order { get; set; }

        // 20 - PrescriptionRequest (1) To (Many) Bid (Has)
        public int PrescriptionRequestId { get; set; }
        public virtual PrescriptionRequest PrescriptionRequest { get; set; }
    }
}
