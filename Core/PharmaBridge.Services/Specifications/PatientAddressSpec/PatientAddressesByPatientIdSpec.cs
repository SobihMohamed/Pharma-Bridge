using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.PatientAddressSpec
{
    public class PatientAddressesByPatientIdSpec : BaseSpecifications<PatientAddress,int>
    {
        public PatientAddressesByPatientIdSpec(string patientId)
            :base(address => address.PatientProfileId == patientId)
        {
            AddOrderBy(address => address.IsDefault, isDescending: true);
        }
    }
}
