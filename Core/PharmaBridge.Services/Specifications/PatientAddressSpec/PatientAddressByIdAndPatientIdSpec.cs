using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.PatientAddressSpec
{
    public class PatientAddressByIdAndPatientIdSpec : BaseSpecifications<PatientAddress, int>
    {
        public PatientAddressByIdAndPatientIdSpec(int addressId, string profileId)
            : base(address => address.Id == addressId && address.PatientProfileId == profileId)
        {

        }
    }
}
