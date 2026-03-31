using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerToUpdateDto
    {
        [Required]
        public string Id { get; set; }
        public string? NationalIdFront { get; set; }
        public string? NationalIdBack { get; set; }
        public string? SyndicateCardImage { get; set; }
    }
}
