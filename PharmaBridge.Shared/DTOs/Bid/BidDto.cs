using PharmaBridge.Shared.DTOs.BidItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class BidDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryFee { get; set; } 

        public string Status { get; set; }
        public string? Notes { get; set; } 
        public List<BidItemDto> BidItems { get; set; } = new List<BidItemDto>();

        public DateTime SubmittedAt { get; set; }
        public int DeliveryTimeInMinutes { get; set; }

        public string PharmacyName { get; set; }
        public decimal PharmacyRating { get; set; }
    }
}
