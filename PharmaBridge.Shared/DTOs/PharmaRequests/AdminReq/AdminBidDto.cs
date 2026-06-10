using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq
{
    public class AdminBidDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public string PharmacyPhone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<AdminBidItemDto> BidItems { get; set; } = new();
    }
}
