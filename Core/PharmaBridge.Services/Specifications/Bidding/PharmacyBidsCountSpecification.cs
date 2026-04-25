
using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.Bid;

namespace PharmaBridge.Services.Specifications.Bidding
{

    public class PharmacyBidsCountSpecification : BaseSpecifications<Bid, int>
    {
        public PharmacyBidsCountSpecification(int pharmacyId, BidQueryParams p)
            : base(b =>
                !b.IsDeleted
                && b.PharmacyId == pharmacyId
                && (!p.Status.HasValue || b.Status == p.Status.Value)
                && (!p.PrescriptionRequestId.HasValue || b.PrescriptionRequestId == p.PrescriptionRequestId.Value)
                && (!p.FromDate.HasValue || b.SubmittedAt >= p.FromDate.Value)
                && (!p.ToDate.HasValue || b.SubmittedAt <= p.ToDate.Value))
        {
        }
    }

}