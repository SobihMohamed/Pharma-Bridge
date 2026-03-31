using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToCreateDto
    {
        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        [StringLength(14, MinimumLength = 14)]
        public string NationalId { get; set; }
        public string? NationalIdFront { get; set; }
        public string? NationalIdBack { get; set; }
        public string? SyndicateCardImage { get; set; }
    }
}
