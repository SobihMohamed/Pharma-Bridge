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
        
        // For the Patient App
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }

        // For the Pharmacy App (so they know whose order this is at a glance)
        public string PatientName { get; set; }
    }
}
