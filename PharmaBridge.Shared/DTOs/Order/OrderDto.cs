using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentMethod { get; set; }

        public bool PaymentStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        // The pharmacy name so the patient can identify the order without tapping into it
        public string PharmacyName { get; set; }
    }
}
