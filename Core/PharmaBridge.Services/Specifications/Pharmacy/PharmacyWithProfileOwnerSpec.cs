using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyWithProfileOwnerSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PharmacyWithProfileOwnerSpec(int pharmacyId) 
            : base(p => p.Id == pharmacyId)
        {
            AddInclude(p => p.PharmaOwner);
            AddInclude(p => p.PharmaOwner.ApplicationUser);
        }
        
        public PharmacyWithProfileOwnerSpec(string ownerId)
            : base(p => p.PharmaOwnerId == ownerId)
        {
            AddInclude(p => p.PharmaOwner);
            AddInclude(p => p.PharmaOwner.ApplicationUser);
        }
    }

    public class PharmaOwnerByAppUserIdSpec : BaseSpecifications<PharmaOwner, string>
    {
        public PharmaOwnerByAppUserIdSpec(string appUserId)
            : base(p => p.ApplicationUserId == appUserId)
        {
            AddInclude(p => p.ApplicationUser);
            AddInclude(p => p.Pharmacies);
        }
    }
}
