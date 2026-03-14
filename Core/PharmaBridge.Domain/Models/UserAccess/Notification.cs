using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Notification :BaseEntity<int>
    {
        public string NotifyType {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceType { get; set; }
        public bool IsRead { get; set; }
    }
}
