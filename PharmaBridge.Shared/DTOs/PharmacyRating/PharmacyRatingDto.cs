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

        // 1. Flattening: Show the name, not the raw GUID
        public string PatientName { get; set; }

        // 2. Metadata: When was this review written?
        public DateTime CreatedAt { get; set; }

        public int OrderId { get; set; }

        public int PharmacyId { get; set; }
        
        public string PharmacyName { get; set; }
    }
}
