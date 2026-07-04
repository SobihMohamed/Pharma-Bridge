using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToCreateDto
    {
        [Required(ErrorMessage = "Pharmacy Name is required")]
        [MinLength(3, ErrorMessage = "Pharmacy Name must be at least 3 characters.")]
        [MaxLength(100)]
        public string PharmacyName { get; set; } = null!;

        [Required(ErrorMessage = "License Number is required")]
        [RegularExpression(@"^\d{3,6}$", ErrorMessage = "License Number must be a sequential number between 3 and 6 digits.")]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = null!;

        [Required(ErrorMessage = "License Image is required")]
        public Microsoft.AspNetCore.Http.IFormFile LicenseImage { get; set; } = null!;

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public decimal Longitude { get; set; }

        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public bool Is24Hours { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Text Address must be at least 3 characters.")]
        [MaxLength(500)]
        public string? TextAddress { get; set; } // not shown to user
        [MaxLength(15)]
        [RegularExpression(@"^[a-zA-Z0-9\u0600-\u06FF\s]+$",
    ErrorMessage = "Area must contain letters and numbers only, no special characters.")]
        [Required]
        public string? Area {get; set;} // which shown in the pharamcyDto for user only
        
        [Phone]
        [MaxLength(11)]
        [Required(ErrorMessage = "Contact phone is required.")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone must be a valid Egyptian number (01xxxxxxxxx).")]

        public string? ContactPhone { get; set; }
    
        // REMOVED: PharmaOwnerId (Security Risk)
        // We get this from the JWT token in the backend.

        // Id         -> auto-generated
        // Status     -> defaults to Pending
        // AverageRating      -> starts at 0.00
        // CompleteOrderCount -> starts at 0
    }
}
