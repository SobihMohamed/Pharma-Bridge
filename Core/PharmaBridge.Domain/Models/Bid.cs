using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models
{
    internal class Bid : BaseEntity<string>
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
        public DateTime? DeliveryTime { get; set; } // ??
    }
}
