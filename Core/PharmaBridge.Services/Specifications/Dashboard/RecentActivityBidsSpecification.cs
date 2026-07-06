using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.Dashboard
{
    public class RecentActivityBidsSpecification : BaseSpecifications<Bid, int>
    {
        public RecentActivityBidsSpecification(int pharmacyId)
            : base(b => b.PharmacyId == pharmacyId
                     && b.Status == BidStatus.Accepted)
        {
            AddInclude(b => b.PrescriptionRequest);
            AddOrderBy(b => b.RespondedAt!, isDescending: true);
            ApplyPaging(PageSize: 10, PageIndex: 1);
        }
    }
}