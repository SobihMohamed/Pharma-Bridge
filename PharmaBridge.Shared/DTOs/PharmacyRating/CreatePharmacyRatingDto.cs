using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmacyRating
{
    public class CreatePharmacyRatingDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int PharmacyId { get; set; }

    }
}
