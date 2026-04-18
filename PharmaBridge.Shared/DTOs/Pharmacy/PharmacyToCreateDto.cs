using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToCreateDto
    {
        [Required(ErrorMessage = "Pharmacy Name is required")]
        [MaxLength(100)]
        public string PharmacyName { get; set; } = null!;

        [Required(ErrorMessage = "License Number is required")]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = null!;

        [Required(ErrorMessage = "License Image is required")]
        public Microsoft.AspNetCore.Http.IFormFile LicenseImage { get; set; } = null!;
    
        [Required]
        public decimal Latitude { get; set; } 
        [Required]
        public decimal Longitude { get; set; }
    
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public bool Is24Hours { get; set; }
    
        [MaxLength(500)]
        public string? TextAddress { get; set; } // not shown to user
        [MaxLength(15)]
        public string? Area {get; set;} // which shown in the pharamcyDto for user only
        
        [Phone]
        [MaxLength(11)]
        public string? ContactPhone { get; set; }
    
        // REMOVED: PharmaOwnerId (Security Risk)
        // We get this from the JWT token in the backend.

        // Id         -> auto-generated
        // Status     -> defaults to Pending
        // AverageRating      -> starts at 0.00
        // CompleteOrderCount -> starts at 0
    }
}
