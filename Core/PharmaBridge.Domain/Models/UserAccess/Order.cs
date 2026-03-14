using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Order : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public string OrderStatus { get; set; }
        public string? CancelReason { get; set; }
        public string PaymentMethod { get; set; }
        public bool PaymentStatus { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime DeliveredAt { get; set; }

    }
}
