using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PendingPharmaciesSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PendingPharmaciesSpec()
            : base(p => p.Status == PharmacyStatus.Pending)
        {
            AddInclude(p => p.PharmaOwner);
            var nestedInclude = nameof(PharmaOwner) + "." + nameof(Domain.Models.User.PharmaOwner.ApplicationUser);
            AddInclude(nestedInclude);
                
            AddOrderBy(p => p.CreatedAt);
        }
    }
}