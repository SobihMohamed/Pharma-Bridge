using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;

namespace PharmaBridge.Services.Specifications.Pharmacy
{
    public class PharmacyByLicenseNumberSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public PharmacyByLicenseNumberSpec(string licenseNumber)
            : base(p => p.LicenseNumber == licenseNumber)
        {
        }
    }
}