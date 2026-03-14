using PharmaBridge.Domain.Models.User;
using System;

namespace PharmaBridge.Domain.Models
{
    public class PharmacyRating : BaseEntity<int>
    {
        public int RatingValue { get; set; }
        public string? Comment { get; set; }

        // 6- PharmacyRating (Many) To (1) pharmacies (Has)
        public int PharmacyId { get; set; }
        public  Pharmacy Pharmacy { get; set; }
    }
}
