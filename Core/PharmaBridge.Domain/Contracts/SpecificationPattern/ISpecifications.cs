using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PharmaBridge.Domain.Contracts.SpecificationPattern
{
    public interface ISpecifications<TEntity,TKey> where TEntity : IEntity<TKey> // select * from table
    {
        // 1 - Where clause => return T or F 
        Expression<Func<TEntity,bool>> Criteria { get; }

        // 2 - Join related entities
        List<Expression<Func<TEntity, object>>> Includes { get; }

        // 3 - OrderBy 
        List<OrderExpressionInfo<TEntity>> OrderByExpressions { get; }

        // 4 - Include strings for navigation properties
        List<string> IncludeStrings { get; }



        // 4 - Pagination
        int Take { get; } // how many records to take 10
        int Skip { get; } // how many records to skip 20
        bool IsPagingEnabled { get; } // to enable pagination or not
    }
}
