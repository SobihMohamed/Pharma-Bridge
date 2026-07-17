using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class GetPatientDetailsByPatientId : BaseSpecifications<Domain.Models.User.PatientProfile, string>
    {
        public GetPatientDetailsByPatientId(string patientId) 
            : base(p => p.Id == patientId)
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
