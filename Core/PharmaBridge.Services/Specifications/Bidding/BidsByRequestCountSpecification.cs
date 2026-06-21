using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.Bid;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public class BidsByRequestCountSpecification : BaseSpecifications<Bid, int>
    {
        public BidsByRequestCountSpecification(int requestId, BidQueryParams p)
            : base(b =>
                !b.IsDeleted
                && b.PrescriptionRequestId == requestId
                && (!p.Status.HasValue || b.Status == p.Status.Value)
                && (!p.FromDate.HasValue || b.SubmittedAt >= p.FromDate.Value)
                && (!p.ToDate.HasValue || b.SubmittedAt <= p.ToDate.Value))
        {
        }
    }
}
