using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToUpdateDto
    {
        // REMOVED: Id (Security: Extract from JWT Token)

        // Basic Info Updates 
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Document Updates
        public string? NationalIdFront { get; set; }
        public string? NationalIdBack { get; set; }
        public string? SyndicateCardImage { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 numeric digits")]
        public string? NationalId {get; set;}
    }
}
