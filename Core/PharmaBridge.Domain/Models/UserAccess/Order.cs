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

        // 15 - PatientProfile (1) To (Many) Orders (Placed)
        public int PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }

        // 16 - Pharmacy (1) To (Many) Order (Get)
        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        // 17 - Order (1) To (1) PrescriptionRequest (Has)
        public string PrescriptionRequestId { get; set; }
        public virtual PrescriptionRequest PrescriptionRequest { get; set; }

        // 19 - Order (1) To (Many) Payment (Pay)
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

        // add the complaints list 
        public virtual ICollection<Complaint> Complaints { get; set; } = new HashSet<Complaint>();
    }
}
