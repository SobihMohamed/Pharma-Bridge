using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientAddress : BaseEntity<int>
    {
        public string AddressLine { get; set; }

        public string City { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public bool IsDefault { get; set; }

        // the address not related to order but related to patient profile and the patient can have many addresses
        // it can in the order table not the address but the order can have a reference to the address that the patient choose when placing the order

        public string PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }
    }

}
