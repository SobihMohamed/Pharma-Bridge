using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq
{
    public class AdminBidItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public bool IsAlternative { get; set; }
        public string? AlternativeNote { get; set; }
    }
}
