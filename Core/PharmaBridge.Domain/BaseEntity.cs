using PharmaBridge.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain
{
    public class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

    }
}
