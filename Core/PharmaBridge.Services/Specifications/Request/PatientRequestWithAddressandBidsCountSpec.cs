using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class PatientRequestWithAddressandBidsCountSpec : BaseSpecifications<PrescriptionRequestEntity,int>
    {
        public PatientRequestWithAddressandBidsCountSpec(string patientId, PrescriptionRequestQueryParams queryParams)
            : base(
                  // manadatory
                  request => request.PatientProfileId == patientId &&
                  // optional filters
                  (!queryParams.Status.HasValue || request.Status == queryParams.Status.Value) &&
                  (!queryParams.FromDate.HasValue || request.CreatedAt >= queryParams.FromDate.Value) &&
                  (!queryParams.ToDate.HasValue || request.CreatedAt <= queryParams.ToDate.Value)
            )
        {
          
        }
    }
}
