using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Payment
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string? GatewayName { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime? SettledAt { get; set; }
        public int OrderId { get; set; }
    }
}
