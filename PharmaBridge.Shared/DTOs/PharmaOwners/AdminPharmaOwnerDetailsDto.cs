using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class AdminPharmaOwnerDetailsDto : PharmaOwnerDetailsDto
    {
        public string? NationalId { get; set; }
        public string? NationalIdFrontUrl { get; set; } 
        public string? NationalIdBackUrl { get; set; }
        public string? SyndicateCardImageUrl { get; set; }
    }
}
