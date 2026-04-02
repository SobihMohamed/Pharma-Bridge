using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmacyRating
{
    public class PharmacyRatingDto
    {
        public int Id { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public string PatientProfileId { get; set; }
        public int PharmacyId { get; set; }
    }
}
