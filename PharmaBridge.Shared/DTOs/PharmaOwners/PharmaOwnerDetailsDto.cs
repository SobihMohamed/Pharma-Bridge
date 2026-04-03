using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerDetailsDto : PharmaOwnerDto
    {
        public string? NationalId { get; set; }
        public string? NationalIdFront { get; set; }
        public string? NationalIdBack { get; set; }
        public string? SyndicateCardImage { get; set; }
    }
}
