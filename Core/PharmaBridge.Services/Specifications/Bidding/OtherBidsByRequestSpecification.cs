using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;


namespace PharmaBridge.Services.Specifications.Bidding
{
    // To reject the remaining bids after accepting one bid.
    public sealed class OtherBidsByRequestSpecification : BaseSpecifications<Bid, int>
    {
        public OtherBidsByRequestSpecification(int prescriptionRequestId, int acceptedBidId)
            : base(b =>
                !b.IsDeleted
                && b.PrescriptionRequestId == prescriptionRequestId
                && b.Id != acceptedBidId
                && b.Status == BidStatus.Pending) // We only reject pending orders; we don't touch previously rejected orders.
        {
        }
    }
}
