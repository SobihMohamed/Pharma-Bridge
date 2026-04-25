using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToUpdateDto
    {
        [MaxLength(100)]
        public string? PharmacyName { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$",
    ErrorMessage = "Area must contain letters only, no numbers or special characters.")]
        public string? GeneralArea { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public decimal? Longitude { get; set; }

        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }

        public bool? Is24Hours { get; set; }

        [MaxLength(500)]
        public string? TextAddress { get; set; }

        [Phone]
        [MaxLength(11)]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone must be a valid Egyptian number (01xxxxxxxxx).")]
        public string? ContactPhone { get; set; }

        public Microsoft.AspNetCore.Http.IFormFile? LicenseImage { get; set; }

        // LicenseNumber -> can update but it require review from admin
        // PharmaOwnerId -> ownership transfer is a separate admin operation
        // Status        -> changed only through dedicated admin endpoints
    }
}