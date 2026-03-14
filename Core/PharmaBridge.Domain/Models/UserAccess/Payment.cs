using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Payment : BaseEntity<int>
    {
        public int PaymentIntendId { get; set; }
        public Datetime? SettedAt { get; set; }
        public string? GatewayName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime InitiateAt { get; set; }
        public  string GatewayResponse {get; set; }

    }
}
