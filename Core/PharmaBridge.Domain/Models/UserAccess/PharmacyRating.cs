using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class PharmacyRating : BaseEntity<int>
    {
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}
