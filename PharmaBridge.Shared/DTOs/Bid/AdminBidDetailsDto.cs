using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class AdminBidDetailsDto :BidDetailsDto
    {
        public decimal PlatformFee { get; set; }

        //admins may need to cross reference with the pharmacy table
        public int PharmacyId { get; set; }

        // Whether a linked order was created from this bid
        // Admins use this to trace the full lifecycle ( Bid → Order )
        public bool HasLinkedOrder { get; set; }

        // The OrderId if the bid was accepted and converted to an order
        // Nullable because bids can be Pending or Rejected (no order yet)
        public int? LinkedOrderId { get; set; }
    }
}
