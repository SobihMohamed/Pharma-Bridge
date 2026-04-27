using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class PatientProfileByAppUserIdSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientProfileByAppUserIdSpec(string applicationUserId)
            : base(p => p.ApplicationUserId == applicationUserId)
        {
        }
    }
}
