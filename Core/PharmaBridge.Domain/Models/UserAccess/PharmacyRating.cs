using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class PharmacyRating : BaseEntity<int>
    {
        public int RatingValue { get; set; }
        public string? Comment { get; set; }

        public int PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public string ApplicationProfileID { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }
    }
}
