using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class BidDto
    {
        public int Id { get; set; }

        public decimal TotalPrice { get; set; }

        // Status as string
        public string Status { get; set; }

        // When the pharmacy submitted the bid patient may prefer the fastest responder
        public DateTime SubmittedAt { get; set; }

        // Estimated delivery time in minutes
        public int DeliveryTimeInMinutes { get; set; }

        // The pharmacy's name is more useful than the PharmacyId integer on a UI card.
        public string PharmacyName { get; set; }

        // Pharmacy rating so the patient can make an informed choice at a glance
        public decimal PharmacyRating { get; set; }
    }
}
