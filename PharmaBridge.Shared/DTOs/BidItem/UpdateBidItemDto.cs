using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.BidItem
{
    public class UpdateBidItemDto
    {
        [Required(ErrorMessage = "BidItem ID is required for update")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [MaxLength(200, ErrorMessage = "Item name cannot exceed 200 characters")]
        public string ItemName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        [Range(1, 9999, ErrorMessage = "Quantity must be between 1 and 9999")]
        public short Quantity { get; set; }

        public bool IsAlternative { get; set; }

        [MaxLength(500, ErrorMessage = "Alternative note cannot exceed 500 characters")]
        public string? AlternativeNote { get; set; }
    }
}
