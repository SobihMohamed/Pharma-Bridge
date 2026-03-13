using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PharmaOwner : BaseEntity<string>
    {
        public string FullName { get; set; }

        public string NationalId { get; set; }

        public string NationalIdFront { get; set; }

        public string NationalIdBack { get; set; }

        public string SyndicateCardImage { get; set; }

        public bool IsVerified { get; set; }
    }
}
