using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class PatientProfileDto
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string FullName { get; set; }
    }
}
