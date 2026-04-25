
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.Bid;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public sealed class AdminBidsSpecification : AdminBidsCountSpecification
    {
        public AdminBidsSpecification(BidQueryParams p) : base(p)
        {
            AddInclude(b => b.BidItems);
            AddInclude(b => b.Pharmacy);
            AddOrderBy(b => b.SubmittedAt, isDescending: true);
            ApplyPaging(p.PageSize, p.PageIndex);
        }
    }
}