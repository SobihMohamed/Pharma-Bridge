using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.Bid;

namespace PharmaBridge.Services.Specifications.Bidding
{
    public class AdminBidsCountSpecification : BaseSpecifications<Bid, int>
    {
        public AdminBidsCountSpecification(BidQueryParams p)
            : base(b =>
                !b.IsDeleted
                && (string.IsNullOrEmpty(p.Search) || (b.Pharmacy != null && b.Pharmacy.PharmacyName.ToLower().Contains((p.Search ?? "").ToLower())))
                && (!p.Status.HasValue || b.Status == p.Status.Value)
                && (!p.PrescriptionRequestId.HasValue || b.PrescriptionRequestId == p.PrescriptionRequestId.Value)
                && (!p.FromDate.HasValue || b.SubmittedAt >= p.FromDate.Value)
                && (!p.ToDate.HasValue || b.SubmittedAt <= p.ToDate.Value))
        {
        }
    }
}