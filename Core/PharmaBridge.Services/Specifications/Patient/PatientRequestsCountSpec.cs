using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientRequestsCountSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public PatientRequestsCountSpec(string patientProfileId)
            : base(r => r.PatientProfileId == patientProfileId)
        {
        }
    }
}
