using PharmaBridge.Shared.DTOs.BidItem;
using System;
using System.Collections.Generic;

namespace PharmaBridge.Shared.DTOs.Order
{
    public class OrderRatingSummaryDto
    {
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
    }

    public class AdminOrderDetailsDto
    {
        public int Id { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Amount { get; set; }

        public string OrderStatus { get; set; }
        public string? CancelReason { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string DeliveryAddress { get; set; }

        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }

        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }

        public int BidId { get; set; }
        public int PrescriptionRequestId { get; set; }

        public OrderRatingSummaryDto? PharmacyRating { get; set; }

        public List<BidItemDto> Items { get; set; } = new();
    }
}