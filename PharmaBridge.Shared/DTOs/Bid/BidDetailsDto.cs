using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class BidDetailsDto
    {
        public int Id { get; set; }

        // Full price breakdown the patient deserves to understand every line before agreeing to pay
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }

        // Both timestamps => patient wants to know when it was submitted and when it was responded to
        public DateTime SubmittedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        public int DeliveryTimeInMinutes { get; set; }

        // The pharmacy name and contact info patient may want to call before accepting
        public string PharmacyName { get; set; }
        public string? PharmacyPhone { get; set; }

        // The full list of items included in this bid
        public List<BidItemDto> BidItems { get; set; } = new();

        // The prescription request this bid belongs to so the UI can provide a back-link
        public int PrescriptionRequestId { get; set; }
    }
}
