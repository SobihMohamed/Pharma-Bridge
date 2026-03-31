using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class AdminPatientProfileDetailsDto : PatientProfileDetailsDto
    {
        public int TotalOrdersCount { get; set; }
        public int TotalComplaintsCount { get; set; }
    }
}
