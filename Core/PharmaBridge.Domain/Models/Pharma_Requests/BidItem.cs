using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class BidItem :BaseEntity<int>
    {
        public string ItemName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; } = 1;
        public bool IsAlternative { get; set; } = false;
        public string? AlternativeNote { get; set; }
        public decimal LineTotal { get; set; }

        // 11 - Bid (1) To (Many) BidItems (Contains)
        public int BidId { get; set; }
        public virtual Bid  Bid { get; set; }
    }
}
