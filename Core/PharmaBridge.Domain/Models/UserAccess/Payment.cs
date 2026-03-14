using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Payment : BaseEntity<int>
    {
        public string PaymentIntendId { get; set; }
        public DateTime? SettledAt { get; set; }
        public string? GatewayName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime InitiateAt { get; set; }
        public  string GatewayResponse {get; set; }

    }
}
