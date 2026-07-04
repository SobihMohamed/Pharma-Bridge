using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Services.Specifications.PharmacySpecs
{
    public class ActivePharmaciesNearLocationSpec : BaseSpecifications<Domain.Models.Pharma_Requests.Pharmacy, int>
    {
        public ActivePharmaciesNearLocationSpec(decimal patientLat, decimal patientLng, decimal radiusInDegrees)
            : base(p =>
                p.Status == PharmacyStatus.Active &&

                p.Latitude >= patientLat - radiusInDegrees && // not more than radius south of the patient
                p.Latitude <= patientLat + radiusInDegrees && // not more than radius north of the patient
                p.Longitude >= patientLng - radiusInDegrees && // not more than radius west of the patient
                p.Longitude <= patientLng + radiusInDegrees // not more than radius east of the patient
            )
        {
            AddInclude(p => p.PharmaOwner);
        }
    }
}