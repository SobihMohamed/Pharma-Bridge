using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Order
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

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

        public string PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }

        public int BidId { get; set; }

        public int PrescriptionRequestId { get; set; }
    }
}
