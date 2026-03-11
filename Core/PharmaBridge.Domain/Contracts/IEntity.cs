using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Contracts
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
