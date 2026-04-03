using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "New order status is required")]
        public string OrderStatus { get; set; }

        // Only required when the status is being set to "Cancelled"

        [MaxLength(500, ErrorMessage = "Cancel reason cannot exceed 500 characters")]
        public string? CancelReason { get; set; }
    }
}
