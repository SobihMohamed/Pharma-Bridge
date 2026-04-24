using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.BidItem
{
    public class BidItemDto
    {
        public string ItemName { get; set; }

        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }

        public bool IsAlternative { get; set; }

        public string? AlternativeNote { get; set; }

        // Pre-calculated total for this line: UnitPrice × Quantity

        public decimal LineTotal { get; set; }

        // The parent bid's Id => useful for frontend navigation/linking
        // public int BidId { get; set; }
    }
}
