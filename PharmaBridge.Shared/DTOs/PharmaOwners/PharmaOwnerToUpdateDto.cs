using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToUpdateDto
    {
        // Basic Info Updates 
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public IFormFile? NationalIdFront { get; set; }
        public IFormFile? NationalIdBack { get; set; }
        public IFormFile? SyndicateCardImage { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 numeric digits")]
        public string? NationalId { get; set; }
    }
}