using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Shared.Common.Params.Bid;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public sealed class BidsByRequestSpecification : BidsByRequestCountSpecification
    {
        public BidsByRequestSpecification(int requestId, BidQueryParams p)
            : base(requestId, p)
        {
            AddInclude(b => b.BidItems);
            AddInclude(b => b.Pharmacy);
            AddOrderBy(b => b.SubmittedAt, isDescending: true);
            ApplyPaging(p.PageSize, p.PageIndex);
        }
    }
}
