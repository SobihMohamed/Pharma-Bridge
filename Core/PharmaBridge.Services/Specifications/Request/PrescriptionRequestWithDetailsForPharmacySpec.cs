using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class PrescriptionRequestWithDetailsForPharmacySpec : BaseSpecifications<PrescriptionRequestEntity,int>
    {
        public PrescriptionRequestWithDetailsForPharmacySpec(int requestId)
            : base(req => req.Id == requestId)
        {
            AddInclude(req => req.Bids);

            AddInclude(req => req.PatientProfile);

            var patientAddress = nameof(PrescriptionRequestEntity.PatientProfile) + "." + nameof(PrescriptionRequestEntity.PatientProfile.PatientAddresses);
            AddInclude(patientAddress);
        }
    }
}
