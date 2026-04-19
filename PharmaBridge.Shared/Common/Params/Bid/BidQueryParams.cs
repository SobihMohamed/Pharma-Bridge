using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Bid
{
    public class BidQueryParams : BaseQueryParam
    {
        // e.g., Pending, Accepted, Rejected, Expired
        public BidStatus? Status { get; set; }

        // To filter bids related to a specific Prescription Request
        public int? PrescriptionRequestId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
