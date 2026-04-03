using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Order ID is required to initiate payment")]
        public int OrderId { get; set; }
    
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } // CreditCard, Wallet, etc.
    
        [MaxLength(100)]
        public string? GatewayName { get; set; } // Stripe, PayPal, Fawry
    
        // REMOVED: Status, Amount, InitiatedAt, PaymentIntentId
        // These are all handled SECURELY on the Server-Side.    }
}
