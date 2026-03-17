using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Payment : BaseEntity<int>
    {
        public string PaymentIntentId { get; set; }
        public DateTime? SettledAt { get; set; }
        public string? GatewayName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public  PaymentStatus Status { get; set; }
        public DateTime InitiatedAt { get; set; }
        public  string? GatewayResponse {get; set; }

        // 19 - Order (1) To (Many) Payment (Pay)
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
