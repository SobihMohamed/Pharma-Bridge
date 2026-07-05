using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.Dashboard
{
    public class ActiveBidsSpecification : BaseSpecifications<Bid, int>
    {
        public ActiveBidsSpecification(int pharmacyId, DateTime from, DateTime to)
            : base(b => b.PharmacyId == pharmacyId
                     && b.Status == BidStatus.Pending
                     && b.SubmittedAt >= from
                     && b.SubmittedAt <= to)
        {
        }
    }
}