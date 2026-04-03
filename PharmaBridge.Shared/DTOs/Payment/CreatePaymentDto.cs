using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
﻿using PharmaBridge.Shared.EnumHelper.PaymentEnums;
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
    
        [Required(ErrorMessage = "Please select a payment method")]
        public PaymentMethodType Method { get; set; } // Enum 
    
        [Required(ErrorMessage = "Payment gateway must be specified")]
        public PaymentGatewayType Gateway { get; set; } // Enum 
    
        // REMOVED: Status, Amount, InitiatedAt, PaymentIntentId
        // These are all handled SECURELY on the Server-Side.    
    }
}
