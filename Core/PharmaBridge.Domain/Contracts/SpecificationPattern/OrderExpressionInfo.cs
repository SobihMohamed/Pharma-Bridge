using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PharmaBridge.Domain.Contracts.SpecificationPattern
{
    public class OrderExpressionInfo<TEntity>
    {
        // example : OrderBy(p => p.Name) => OrderByExpression = p => p.Name , IsDescending = false
        public Expression<Func<TEntity, object>>? OrderByExpression { get; set; }
        public bool IsDescending { get; set; }
    }
}
