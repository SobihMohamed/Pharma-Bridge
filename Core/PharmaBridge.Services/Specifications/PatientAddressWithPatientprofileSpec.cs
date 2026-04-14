using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class PatientAddressWithPatientprofileSpec : BaseSpecifications<PatientAddress,int>
    {
        public PatientAddressWithPatientprofileSpec(int addressId , string PatientId) 
            :base(a => a.Id == addressId && a.PatientProfileId == PatientId)
        {
            
        }
    }
}
