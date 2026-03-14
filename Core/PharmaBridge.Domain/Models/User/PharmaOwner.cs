using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PharmaOwner : BaseEntity<string>
    {
        public string FullName { get; set; }

        public string? NationalId { get; set; }

        public string? NationalIdFront { get; set; }

        public string? NationalIdBack { get; set; }

        public string? SyndicateCardImage { get; set; }

        public PharmaOwnerStatus Status { get; set; } = PharmaOwnerStatus.Pending;
    }
}
