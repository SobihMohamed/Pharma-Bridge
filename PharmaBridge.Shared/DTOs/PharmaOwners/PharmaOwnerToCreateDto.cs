using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; 

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToCreateDto
    {
        [Required(ErrorMessage = "National ID is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 numeric digits")]
        public string NationalId { get; set; } = null!;

        [Required(ErrorMessage = "Front image of National ID is required")]
        public IFormFile NationalIdFront { get; set; } = null!;

        [Required(ErrorMessage = "Back image of National ID is required")]
        public IFormFile NationalIdBack { get; set; } = null!;

        [Required(ErrorMessage = "Syndicate Card image is required")]
        public IFormFile SyndicateCardImage { get; set; } = null!;
    }
}