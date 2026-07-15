using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientRatingsCountSpec : BaseSpecifications<PharmacyRating, int>
    {
        public PatientRatingsCountSpec(string patientProfileId)
            : base(r => r.PatientProfileId == patientProfileId)
        {
        }
    }
}
