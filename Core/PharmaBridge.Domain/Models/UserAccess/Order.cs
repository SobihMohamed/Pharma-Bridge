using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Order : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? CancelReason { get; set; }
        public string PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // 13 - Order (1) To (1) PatientAddress (Go To)
        public int PatientAddressId { get; set; }
        public virtual PatientAddress PatientAddress { get; set; }

        // 14 - Order (1) To (1) Bid (Converted To)
        public int BidId { get; set; }
        public virtual Bid Bid { get; set; }
    }
}
