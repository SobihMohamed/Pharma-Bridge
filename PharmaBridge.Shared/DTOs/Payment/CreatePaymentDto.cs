using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        [MaxLength(100)]
        public string PaymentIntentId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public string PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [MaxLength(100)]
        public string? GatewayName { get; set; }
        [MaxLength(500)]
        public string? GatewayResponse { get; set; }
        public DateTime InitiatedAt { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
