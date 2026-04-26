using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmacyEntity = PharmaBridge.Domain.Models.Pharma_Requests.Pharmacy; 

namespace PharmaBridge.Services.Specifications.PharmacySpec
{
    public class PharmacyByOwnerAppUserIdSpec : BaseSpecifications<PharmacyEntity, int>
    {
        public PharmacyByOwnerAppUserIdSpec(string applicationUserId)
            : base(p => p.PharmaOwner.ApplicationUserId == applicationUserId
                     && p.Status == PharmacyStatus.Active)
        {
            AddInclude(p => p.PharmaOwner);
        }
    }
}