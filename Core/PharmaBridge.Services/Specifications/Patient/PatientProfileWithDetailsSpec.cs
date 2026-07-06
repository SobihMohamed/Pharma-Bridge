using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientProfileWithDetailsSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientProfileWithDetailsSpec(string patientProfileId)
             : base(p => p.Id == patientProfileId)
        {
            AddInclude(p => p.ApplicationUser);

            // NEW — required for the new fields
            AddInclude(p => p.PatientAddresses);
            AddInclude(p => p.Orders);
            AddInclude(p => p.PrescriptionRequests);
            AddInclude(p => p.PharmacyRatings);
            AddInclude($"{nameof(PatientProfile.ApplicationUser)}.{nameof(ApplicationUser.Complaints)}");
        }
    }
}
