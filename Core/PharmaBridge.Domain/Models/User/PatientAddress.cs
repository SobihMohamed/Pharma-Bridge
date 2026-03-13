using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientAddress : BaseEntity<string>
    {
        public string AddressLine { get; set; }

        public string City { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsDefault { get; set; }
    }
    
}
