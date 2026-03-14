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

        // 4- ApplicationUser (1) To (1) Pharma_Owner
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        // 8- pharmacies (Many) To (1) Pharma_Owner (owned)
        public  ICollection<Pharmacy> OwnedPharmacies { get; set; } = new HashSet<Pharmacy>();
    }
}
