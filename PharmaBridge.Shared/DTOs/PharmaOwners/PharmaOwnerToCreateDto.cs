using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToCreateDto
    {
       // REMOVED: ApplicationUserId (Security: Get it from JWT Token)

        [Required(ErrorMessage = "National ID is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID must be exactly 14 digits")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Front image of National ID is required")]
        public string NationalIdFront { get; set; }

        [Required(ErrorMessage = "Back image of National ID is required")]
        public string NationalIdBack { get; set; }

        [Required(ErrorMessage = "Syndicate Card image is required")]
        public string SyndicateCardImage { get; set; }
    }
}
