using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.PatientAddressSpec
{
    public class PatientProfileByAppUserIdSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientProfileByAppUserIdSpec(string applicationUserId)
            : base(profile => profile.ApplicationUserId == applicationUserId)
        {
        }
    }
}