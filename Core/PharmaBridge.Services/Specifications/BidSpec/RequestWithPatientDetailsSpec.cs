using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.BidSpec
{
    public class RequestWithPatientDetailsSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public RequestWithPatientDetailsSpec(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.PatientProfile);
            // include application user details
            var appUserInclude = nameof(PrescriptionRequestEntity.PatientProfile) + "." + nameof(PrescriptionRequestEntity.PatientProfile.ApplicationUser);
            AddInclude(appUserInclude); 
        }
    }
}
