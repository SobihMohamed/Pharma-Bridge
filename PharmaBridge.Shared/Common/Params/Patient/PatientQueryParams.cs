using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Patient
{
    public class PatientQueryParams : BaseQueryParam
    {
        // To search by Patient Name, Email, or Phone
        public string? SearchTerm { get; set; }
    }
}
