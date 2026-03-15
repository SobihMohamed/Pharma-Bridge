using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Domain.Models.User
{
    public class PharmaOwner : BaseEntity<string>
    {

        public string? NationalId { get; set; }

        public string? NationalIdFront { get; set; }

        public string? NationalIdBack { get; set; }

        public string? SyndicateCardImage { get; set; }

        public PharmaOwnerStatus Status { get; set; } = PharmaOwnerStatus.Pending;

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; } =new HashSet<Pharmacy>();
    }
}
