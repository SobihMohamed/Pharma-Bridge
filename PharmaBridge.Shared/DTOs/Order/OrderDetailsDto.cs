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
        public string PaymentStatus { get; set; }

        // Lifecycle timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string DeliveryAddress { get; set; }

        // 2. Pharmacy Info
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }

        // 3. Patient Info
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }

        public int BidId { get; set; }
        public int PrescriptionRequestId { get; set; }

        public OrderRatingSummaryDto? PharmacyRating { get; set; }

        // 4. The actual items ordered!
        public List<BidItemDto> Items { get; set; } = new();
    }
}