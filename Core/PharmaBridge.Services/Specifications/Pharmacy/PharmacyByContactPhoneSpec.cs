using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyByContactPhoneSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PharmacyByContactPhoneSpec(string contactPhone)
            : base(p => p.ContactPhone == contactPhone)
        {
        }
    }
}