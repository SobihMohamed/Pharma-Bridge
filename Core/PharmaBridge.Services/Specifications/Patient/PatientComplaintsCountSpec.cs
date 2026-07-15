using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientComplaintsCountSpec : BaseSpecifications<Complaint, int> 
    {
        public PatientComplaintsCountSpec(string patientProfileId)
            : base(c => c.SubmittedById == patientProfileId)
        {
        }
    }
}
