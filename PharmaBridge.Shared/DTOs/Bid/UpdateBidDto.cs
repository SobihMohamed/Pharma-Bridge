using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class UpdateBidDto
    {

        [Required(ErrorMessage = "Bid ID is required for update")]
        public int Id { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than zero")]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount amount cannot be negative")]
        public decimal DiscountAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Delivery fee cannot be negative")]
        public decimal DeliveryFee { get; set; }

        [Range(1, 1440, ErrorMessage = "Delivery time must be between 1 and 1440 minutes")]
        public int DeliveryTimeInMinutes { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        // Pharmacy may update the item list (e.g., correct a quantity or price)
        // Nullable list — if null, keep existing items unchanged
        public List<UpdateBidItemDto>? BidItems { get; set; }
    }
}
