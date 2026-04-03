using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Bid
{
    public class AdminBidDetailsDto :BidDetailsDto
    {
        public decimal PlatformFee { get; set; }
    }
}
