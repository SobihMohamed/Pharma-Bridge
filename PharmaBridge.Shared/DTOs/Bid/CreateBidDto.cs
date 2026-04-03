using PharmaBridge.Shared.DTOs.BidItem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class CreateBidDto
    {
        // [Required] a bid without a prescription target is meaningless

        [Required(ErrorMessage = "PrescriptionRequestId is required")]
        public int PrescriptionRequestId { get; set; }

        // Which pharmacy is submitting the bid

        [Required(ErrorMessage = "PharmacyId is required")]
        public int PharmacyId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than zero")]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount amount cannot be negative")]
        public decimal DiscountAmount { get; set; } = 0.00m;

        [Range(0, double.MaxValue, ErrorMessage = "Delivery fee cannot be negative")]
        public decimal DeliveryFee { get; set; } = 0.00m;

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required]
        [Range(1, 1440, ErrorMessage = "Delivery time must be between 1 and 1440 minutes (24 hours)")]
        public int DeliveryTimeInMinutes { get; set; }

        // The list of items in this bid. A bid with no items is invalid
        [Required(ErrorMessage = "A bid must contain at least one item")]
        public List<CreateBidItemDto> BidItems { get; set; } = new();
    }
}
