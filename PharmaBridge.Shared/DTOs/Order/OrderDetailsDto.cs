using PharmaBridge.Shared.DTOs.BidItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Order
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

       // 1. Price Breakdown (Flattened from Bid)
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Amount { get; set; } // This is the Total Price

        public string OrderStatus { get; set; }

        public string? CancelReason { get; set; }

        public string PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }

        // Lifecycle timestamps — patient wants to know when key events happened
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string DeliveryAddress { get; set; }

        // 2. Pharmacy Info (For the Patient)
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }

        // 3. Patient Info (For the Pharmacy & Delivery Guy)
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        
        public int BidId { get; set; }
        public int PrescriptionRequestId { get; set; }
        
        // 4. The actual items ordered! (Mapped from Bid.BidItems)
        public List<BidItemDto> Items { get; set; } = new();
    }
}
